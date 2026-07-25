/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at
 * https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

import { CubismMatrix44 } from '@framework/math/cubismmatrix44';
import { CubismViewMatrix } from '@framework/math/cubismviewmatrix';

import * as LAppDefine from './lappdefine';
import { LAppPal } from './lapppal';
import { TouchManager } from './touchmanager';
import { LAppSubdelegate } from './lappsubdelegate';

/**
 * Quản lý vùng hiển thị và tương tác với model Live2D.
 */
export class LAppView {
  public constructor() {
    this._touchManager = new TouchManager();
    this._deviceToScreen = new CubismMatrix44();
    this._viewMatrix = new CubismViewMatrix();
  }

  /**
   * Khởi tạo ma trận hiển thị.
   */
  public initialize(subdelegate: LAppSubdelegate): void {
    this._subdelegate = subdelegate;

    const { width, height } = subdelegate.getCanvas();

    const ratio: number = width / height;
    const left: number = -ratio;
    const right: number = ratio;
    const bottom: number = LAppDefine.ViewLogicalLeft;
    const top: number = LAppDefine.ViewLogicalRight;

    this._viewMatrix.setScreenRect(left, right, bottom, top);
    this._viewMatrix.scale(
      LAppDefine.ViewScale,
      LAppDefine.ViewScale
    );

    this._deviceToScreen.loadIdentity();

    if (width > height) {
      const screenW: number = Math.abs(right - left);

      this._deviceToScreen.scaleRelative(
        screenW / width,
        -screenW / width
      );
    } else {
      const screenH: number = Math.abs(top - bottom);

      this._deviceToScreen.scaleRelative(
        screenH / height,
        -screenH / height
      );
    }

    this._deviceToScreen.translateRelative(
      -width * 0.5,
      -height * 0.5
    );

    this._viewMatrix.setMaxScale(LAppDefine.ViewMaxScale);
    this._viewMatrix.setMinScale(LAppDefine.ViewMinScale);

    this._viewMatrix.setMaxScreenRect(
      LAppDefine.ViewLogicalMaxLeft,
      LAppDefine.ViewLogicalMaxRight,
      LAppDefine.ViewLogicalMaxBottom,
      LAppDefine.ViewLogicalMaxTop
    );
  }

  /**
   * Giải phóng tài nguyên của view.
   */
  public release(): void {
    this._viewMatrix = null;
    this._touchManager = null;
    this._deviceToScreen = null;
    this._subdelegate = null;
  }

  /**
   * Render model Live2D.
   */
  public render(): void {
    const live2DManager = this._subdelegate?.getLive2DManager();

    if (live2DManager == null) {
      return;
    }

    live2DManager.setViewMatrix(this._viewMatrix);
    live2DManager.onUpdate();
  }

  /**
   * Demo trước đây khởi tạo background và nút Gear tại đây.
   * AikoOS không sử dụng các sprite giao diện đó.
   */
  public initializeSprite(): void {
    // Không cần khởi tạo sprite.
  }

  /**
   * Bắt đầu thao tác chuột hoặc cảm ứng.
   */
  public onTouchesBegan(pointX: number, pointY: number): void {
    this._touchManager.touchesBegan(
      pointX * window.devicePixelRatio,
      pointY * window.devicePixelRatio
    );
  }

  /**
   * Di chuyển chuột hoặc cảm ứng.
   */
  public onTouchesMoved(pointX: number, pointY: number): void {
    const posX = pointX * window.devicePixelRatio;
    const posY = pointY * window.devicePixelRatio;

    this._touchManager.touchesMoved(posX, posY);

    const viewX: number = this.transformViewX(
      this._touchManager.getX()
    );

    const viewY: number = this.transformViewY(
      this._touchManager.getY()
    );

    const live2DManager = this._subdelegate.getLive2DManager();

    live2DManager?.onDrag(viewX, viewY);
  }

  /**
   * Kết thúc thao tác chuột hoặc cảm ứng.
   */
  public onTouchesEnded(pointX: number, pointY: number): void {
    const posX = pointX * window.devicePixelRatio;
    const posY = pointY * window.devicePixelRatio;

    const live2DManager = this._subdelegate.getLive2DManager();

    if (live2DManager == null) {
      return;
    }

    // Trả hướng nhìn về trạng thái trung tâm.
    live2DManager.onDrag(0.0, 0.0);

    const x: number = this.transformViewX(posX);
    const y: number = this.transformViewY(posY);

    if (LAppDefine.DebugTouchLogEnable) {
      LAppPal.printMessage(
        `[AikoOS] touchesEnded x: ${x} y: ${y}`
      );
    }

    // Giữ tương tác chạm vào Mao để chạy motion/expression.
    live2DManager.onTap(x, y);
  }

  /**
   * Chuyển tọa độ thiết bị X sang tọa độ View.
   */
  public transformViewX(deviceX: number): number {
    const screenX: number =
      this._deviceToScreen.transformX(deviceX);

    return this._viewMatrix.invertTransformX(screenX);
  }

  /**
   * Chuyển tọa độ thiết bị Y sang tọa độ View.
   */
  public transformViewY(deviceY: number): number {
    const screenY: number =
      this._deviceToScreen.transformY(deviceY);

    return this._viewMatrix.invertTransformY(screenY);
  }

  /**
   * Chuyển tọa độ thiết bị X sang tọa độ Screen.
   */
  public transformScreenX(deviceX: number): number {
    return this._deviceToScreen.transformX(deviceX);
  }

  /**
   * Chuyển tọa độ thiết bị Y sang tọa độ Screen.
   */
  public transformScreenY(deviceY: number): number {
    return this._deviceToScreen.transformY(deviceY);
  }

  private _touchManager: TouchManager;
  private _deviceToScreen: CubismMatrix44;
  private _viewMatrix: CubismViewMatrix;
  private _subdelegate: LAppSubdelegate;
}