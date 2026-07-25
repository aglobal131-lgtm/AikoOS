import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";

export default defineConfig({
    base: "./",

    build: {
        outDir: fileURLToPath(
            new URL("../Web", import.meta.url)
        ),

        emptyOutDir: true
    }
});