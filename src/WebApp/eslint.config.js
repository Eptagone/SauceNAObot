import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import stylistic from "@stylistic/eslint-plugin";
import tsdoc from "eslint-plugin-tsdoc";

export default tseslint.config(
    eslint.configs.recommended,
    ...tseslint.configs.recommendedTypeChecked,
    {
        plugins: {
            tsdoc,
        },
        languageOptions: {
            parserOptions: {
                projectService: true,
                tsconfigRootDir: import.meta.dirname,
            },
        },
    },
    stylistic.configs.customize({
        flat: true,
        indent: 4,
        quotes: "double",
        semi: true,
    }),
    {
        rules: {
            "tsdoc/syntax": "warn",
            "no-case-declarations": "off",
            "@typescript-eslint/no-unused-vars": [
                "error",
                {
                    varsIgnorePattern: "^_",
                    argsIgnorePattern: "^_",
                    caughtErrorsIgnorePattern: "^_",
                },
            ],
            "@typescript-eslint/no-empty-object-type": "warn",
            "@typescript-eslint/no-floating-promises": "off",
            "@typescript-eslint/no-misused-promises": "off",
            "@typescript-eslint/no-unnecessary-condition": "warn",
            "@typescript-eslint/no-unsafe-argument": "off",
            "@typescript-eslint/no-unsafe-assignment": "off",
            "@typescript-eslint/no-unsafe-member-access": "off",
            "@typescript-eslint/no-unsafe-return": "off",
            "@typescript-eslint/prefer-find": "warn",
            "@typescript-eslint/require-await": "warn",
            "@typescript-eslint/restrict-template-expressions": "off",
            "@typescript-eslint/triple-slash-reference": "off",
            "@stylistic/arrow-parens": "warn",
            "@stylistic/arrow-spacing": "warn",
            "@stylistic/brace-style": "warn",
            "@stylistic/comma-dangle": "warn",
            "@stylistic/comma-spacing": "warn",
            "@stylistic/eol-last": "warn",
            "@stylistic/indent-binary-ops": "warn",
            "@stylistic/indent": "off",
            "@stylistic/key-spacing": "warn",
            "@stylistic/keyword-spacing": "warn",
            "@stylistic/max-statements-per-line": "warn",
            "@stylistic/multiline-ternary": "off",
            "@stylistic/no-multi-spaces": "warn",
            "@stylistic/no-multiple-empty-lines": "warn",
            "@stylistic/no-trailing-spaces": "warn",
            "@stylistic/object-curly-spacing": "warn",
            "@stylistic/operator-linebreak": "warn",
            "@stylistic/padded-blocks": "warn",
            "@stylistic/space-before-blocks": "warn",
            "@stylistic/quote-props": "warn",
            "@stylistic/quotes": "warn",
            "@stylistic/space-in-parens": "warn",
            "@stylistic/type-annotation-spacing": "warn",
        },
    },
);
