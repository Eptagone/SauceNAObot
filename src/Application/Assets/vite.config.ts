import tailwindcss from "@tailwindcss/vite";
import type { UserConfig } from "vite";

const base = process.env.BASE_PATH || "/dist/";

export default {
    appType: "custom",
    publicDir: false,
    base,
    root: "../",
    build: {
        manifest: true,
        outDir: "wwwroot/dist",
        rollupOptions: {
            input: "main.css",
        },
    },
    plugins: [tailwindcss()],
    optimizeDeps: {
        include: [],
    },
} satisfies UserConfig;
