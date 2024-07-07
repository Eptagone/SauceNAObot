/**
 * Name: vite.config.ts
 * Description: Vite configuration file.
 */

import type { UserConfig } from "vite";
import tailwindcss from "tailwindcss";

export default {
    appType: 'custom',
    publicDir: false,
    base: "/dist/",
    build: {
        manifest: true,
        assetsDir: ".",
        outDir: "wwwroot/dist",
        rollupOptions: {
            input: "Assets/style.css",
        }
    },
    css: {
        postcss: {
            plugins: [tailwindcss()]
        }
    },
    optimizeDeps: {
        include: []
    }
} satisfies UserConfig;
