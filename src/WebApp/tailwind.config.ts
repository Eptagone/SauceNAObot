/**
 * Name: tailwind.config.ts
 * Description: Tailwind configuration file.
 */

import type { Config } from "tailwindcss";

export default {
    content: [
        "./Components/**/*.razor",
    ],
    theme: {
        extend: {},
    },
    plugins: [],
} satisfies Config;
