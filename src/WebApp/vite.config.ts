import type { UserConfig } from "vite";
import tailwindcss from "@tailwindcss/vite";

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
