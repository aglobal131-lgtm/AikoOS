import { getActiveLive2DManager } from "./lapplive2dmanager";

export class AikoBridge {

    public playMotion(group: string, index: number): boolean {

        const manager = getActiveLive2DManager();

        if (!manager) {
            console.warn("[AikoBridge] Live2D manager chưa sẵn sàng.");
            return false;
        }

        return manager.playMotion(group, index);
    }

    public setExpression(name: string): boolean {

        const manager = getActiveLive2DManager();

        if (!manager) {
            console.warn("[AikoBridge] Live2D manager chưa sẵn sàng.");
            return false;
        }

        return manager.setExpression(name);
    }

    public lookAt(x: number, y: number): boolean {

        const manager = getActiveLive2DManager();

        if (!manager) {
            console.warn("[AikoBridge] Live2D manager chưa sẵn sàng.");
            return false;
        }

        return manager.lookAt(x, y);
    }

    public getMotionGroups(): string[] {
  const manager = getActiveLive2DManager();

  if (!manager) {
    console.warn('[AikoBridge] Live2D manager chưa sẵn sàng.');
    return [];
  }

  return manager.getMotionGroups();
}

public getExpressions(): string[] {
  const manager = getActiveLive2DManager();

  if (!manager) {
    console.warn('[AikoBridge] Live2D manager chưa sẵn sàng.');
    return [];
  }

  return manager.getExpressions();
}
}

declare global {

    interface Window {

        Aiko: AikoBridge;

    }

}

window.Aiko = new AikoBridge();

export {};