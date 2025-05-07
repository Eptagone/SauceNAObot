import tailwindcss from "@tailwindcss/vite";
import type { UserConfig } from "vite";

export default {
    appType: "custom",
    publicDir: false,
    base: "/dist/",
    build: {
        manifest: true,
        outDir: "wwwroot/dist",
        rollupOptions: {
            input: "tailwind.config.css",
        },
    },
    plugins: [tailwindcss()],
    optimizeDeps: {
        include: [],
    },
} satisfies UserConfig;
