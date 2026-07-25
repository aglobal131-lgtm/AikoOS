/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

import { CubismMatrix44 } from '@framework/math/cubismmatrix44';
import { ACubismMotion } from '@framework/motion/acubismmotion';
import { InvalidMotionQueueEntryHandleValue } from '@framework/motion/cubismmotionqueuemanager';
import { CubismWebGLOffscreenManager } from '@framework/rendering/cubismoffscreenmanager';

import * as LAppDefine from './lappdefine';
import { LAppModel } from './lappmodel';
import { LAppPal } from './lapppal';
import { LAppSubdelegate } from './lappsubdelegate';

/**
 * Manager Live2D đang hoạt động.
 * AikoBridge sẽ lấy manager thông qua hàm getActiveLive2DManager().
 */
let activeLive2DManager: LAppLive2DManager | null = null;

/**
 * Lấy Live2D manager hiện đang hoạt động.
 */
export function getActiveLive2DManager(): LAppLive2DManager | null {
  return activeLive2DManager;
}

/**
 * Class quản lý CubismModel trong ứng dụng.
 *
 * Chịu trách nhiệm:
 * - Tạo và giải phóng model.
 * - Cập nhật và vẽ model.
 * - Xử lý drag và tap.
 * - Cung cấp API điều khiển cho AikoBridge.
 */
export class LAppLive2DManager {
  /**
   * Giải phóng toàn bộ model đang được giữ.
   */
  private releaseAllModel(): void {
    for (const model of this._models) {
      model.release();
    }

    this._models.length = 0;
  }

  /**
   * Thay đổi kích thước render target.
   */
  public setOffscreenSize(width: number, height: number): void {
    for (let i = 0; i < this._models.length; i++) {
      const model: LAppModel = this._models[i];

      if (model) {
        model.setRenderTargetSize(width, height);
      }
    }
  }

  /**
   * Xử lý khi người dùng kéo chuột.
   *
   * @param x Tọa độ X đã chuẩn hóa.
   * @param y Tọa độ Y đã chuẩn hóa.
   */
  public onDrag(x: number, y: number): void {
    const model: LAppModel | undefined = this._models[0];

    if (model) {
      model.setDragging(x, y);
    }
  }

  /**
   * Xử lý khi người dùng nhấn vào model.
   *
   * @param x Tọa độ X.
   * @param y Tọa độ Y.
   */
  public onTap(x: number, y: number): void {
    if (LAppDefine.DebugLogEnable) {
      LAppPal.printMessage(
        `[APP]tap point: {x: ${x.toFixed(2)} y: ${y.toFixed(2)}}`
      );
    }

    const model: LAppModel | undefined = this._models[0];

    if (!model) {
      return;
    }

    if (model.hitTest(LAppDefine.HitAreaNameHead, x, y)) {
      if (LAppDefine.DebugLogEnable) {
        LAppPal.printMessage(
          `[APP]hit area: [${LAppDefine.HitAreaNameHead}]`
        );
      }

      model.setRandomExpression();
      return;
    }

    if (model.hitTest(LAppDefine.HitAreaNameBody, x, y)) {
      if (LAppDefine.DebugLogEnable) {
        LAppPal.printMessage(
          `[APP]hit area: [${LAppDefine.HitAreaNameBody}]`
        );
      }

      model.startRandomMotion(
        LAppDefine.MotionGroupTapBody,
        LAppDefine.PriorityNormal,
        this.finishedMotion,
        this.beganMotion
      );
    }
  }

  /**
   * Cập nhật và vẽ model trong mỗi frame.
   */
  public onUpdate(): void {
    const gl = this._subdelegate.getGl();

    CubismWebGLOffscreenManager.getInstance().beginFrameProcess(gl);

    const { width, height } = this._subdelegate.getCanvas();

    const projection = new CubismMatrix44();
    const model: LAppModel | undefined = this._models[0];

    if (!model) {
      CubismWebGLOffscreenManager.getInstance().endFrameProcess(gl);
      return;
    }

    const cubismModel = model.getModel();

    if (cubismModel) {
      if (cubismModel.getCanvasWidth() > 1.0 && width < height) {
        model.getModelMatrix().setWidth(2.0);
        projection.scale(1.0, width / height);
      } else {
        projection.scale(height / width, 1.0);
      }

      if (this._viewMatrix != null) {
        projection.multiplyByMatrix(this._viewMatrix);
      }
    }

    model.update();
    model.draw(projection);

    CubismWebGLOffscreenManager.getInstance().endFrameProcess(gl);

    CubismWebGLOffscreenManager.getInstance().releaseStaleRenderTextures(gl);
  }

  /**
   * Chuyển sang scene tiếp theo.
   *
   * Hiện tại AikoOS chỉ sử dụng một model nên method này chưa làm gì.
   */
  public nextScene(): void {
    return;
  }

  /**
   * Đổi model theo chỉ số scene.
   *
   * @param index Chỉ số model trong ModelDir.
   */
  private changeScene(index: number): void {
    this._sceneIndex = index;

    if (LAppDefine.DebugLogEnable) {
      LAppPal.printMessage(`[APP]model index: ${this._sceneIndex}`);
    }

    const modelName: string | undefined = LAppDefine.ModelDir[index];

    if (!modelName) {
      console.error(
        `[Live2DManager] Model index không hợp lệ: ${index}`
      );
      return;
    }

    const modelPath = `${LAppDefine.ResourcesPath}${modelName}/`;
    const modelJsonName = `${modelName}.model3.json`;

    this.releaseAllModel();

    const instance = new LAppModel();

    instance.setSubdelegate(this._subdelegate);
    instance.loadAssets(modelPath, modelJsonName);

    this._models.push(instance);
  }

  /**
   * Sao chép view matrix vào manager.
   */
  public setViewMatrix(matrix: CubismMatrix44): void {
    const source = matrix.getArray();
    const destination = this._viewMatrix.getArray();

    for (let i = 0; i < 16; i++) {
      destination[i] = source[i];
    }
  }

  /**
   * Thêm model theo scene index.
   */
  public addModel(sceneIndex: number = 0): void {
    this._sceneIndex = sceneIndex;
    this.changeScene(this._sceneIndex);
  }

  /**
   * Phát một motion cụ thể.
   *
   * @param group Tên motion group, ví dụ Idle.
   * @param index Chỉ số motion trong group.
   * @returns true nếu motion được bắt đầu thành công.
   */
  public playMotion(group: string, index: number): boolean {
    const model: LAppModel | undefined = this._models[0];

    if (!model) {
      console.warn('[Live2DManager] Model chưa tồn tại.');
      return false;
    }

    if (!model._modelSetting) {
      console.warn('[Live2DManager] Model chưa tải xong.');
      return false;
    }

    const motionCount = model._modelSetting.getMotionCount(group);

    if (motionCount <= 0) {
      console.warn(
        `[Live2DManager] Không tìm thấy motion group: ${group}`
      );
      return false;
    }

    if (!Number.isInteger(index) || index < 0 || index >= motionCount) {
      console.warn(
        `[Live2DManager] Motion index không hợp lệ: ${group}[${index}]`
      );
      return false;
    }

    const handle = model.startMotion(
      group,
      index,
      LAppDefine.PriorityForce,
      this.finishedMotion,
      this.beganMotion
    );

    return handle !== InvalidMotionQueueEntryHandleValue;
  }

  /**
   * Đặt biểu cảm theo tên.
   *
   * @param expressionName Tên expression trong model3.json.
   * @returns true nếu expression tồn tại và được áp dụng.
   */
  public setExpression(expressionName: string): boolean {
    const model: LAppModel | undefined = this._models[0];

    if (!model) {
      console.warn('[Live2DManager] Model chưa tồn tại.');
      return false;
    }

    if (!model._expressions.has(expressionName)) {
      console.warn(
        `[Live2DManager] Không tìm thấy expression: ${expressionName}`
      );
      return false;
    }

    model.setExpression(expressionName);

    return true;
  }

  /**
   * Điều khiển hướng nhìn của model.
   *
   * x và y nên nằm trong khoảng từ -1 đến 1.
   *
   * @param x Hướng nhìn theo chiều ngang.
   * @param y Hướng nhìn theo chiều dọc.
   */
  public lookAt(x: number, y: number): boolean {
    const model: LAppModel | undefined = this._models[0];


    
    if (!model) {
      console.warn('[Live2DManager] Model chưa tồn tại.');
      return false;
    }

    if (!Number.isFinite(x) || !Number.isFinite(y)) {
      console.warn(
        '[Live2DManager] Tọa độ lookAt phải là số hợp lệ.'
      );
      return false;
    }

    const normalizedX = Math.max(-1, Math.min(1, x));
    const normalizedY = Math.max(-1, Math.min(1, y));

    model.setDragging(normalizedX, normalizedY);

    return true;
  }

  /**
 * Lấy danh sách nhóm motion của model.
 */
public getMotionGroups(): string[] {
  const model = this._models[0];

  if (!model || !model._modelSetting) {
    return [];
  }

  const groups: string[] = [];
  const groupCount = model._modelSetting.getMotionGroupCount();

  for (let i = 0; i < groupCount; i++) {
    groups.push(model._modelSetting.getMotionGroupName(i));
  }

  return groups;
}

/**
 * Lấy danh sách expression đã được tải.
 */
public getExpressions(): string[] {
  const model = this._models[0];

  if (!model) {
    return [];
  }

  return Array.from(model._expressions.keys());
}

  /**
   * Constructor.
   */
  public constructor() {
    this._subdelegate = null;
    this._viewMatrix = new CubismMatrix44();
    this._models = new Array<LAppModel>();
    this._sceneIndex = 0;
  }

  /**
   * Giải phóng manager.
   */
  public release(): void {
    if (activeLive2DManager === this) {
      activeLive2DManager = null;
    }

    this.releaseAllModel();
  }

  /**
   * Khởi tạo manager.
   *
   * @param subdelegate Subdelegate sở hữu manager này.
   */
  public initialize(subdelegate: LAppSubdelegate): void {
    this._subdelegate = subdelegate;

    activeLive2DManager = this;

    this.changeScene(this._sceneIndex);
  }

  /**
   * Callback khi motion bắt đầu.
   */
  public beganMotion = (motion: ACubismMotion): void => {
    LAppPal.printMessage('Motion Began:');
    console.log(motion);
  };

  /**
   * Callback khi motion kết thúc.
   */
  public finishedMotion = (motion: ACubismMotion): void => {
    LAppPal.printMessage('Motion Finished:');
    console.log(motion);
  };

  /**
   * Subdelegate đang sở hữu manager.
   */
  private _subdelegate: LAppSubdelegate;

  /**
   * View matrix dùng để vẽ model.
   */
  private _viewMatrix: CubismMatrix44;

  /**
   * Danh sách model đang được quản lý.
   */
  private _models: Array<LAppModel>;

  /**
   * Chỉ số scene hiện tại.
   */
  private _sceneIndex: number;
}