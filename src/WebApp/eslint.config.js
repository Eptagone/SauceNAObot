import eslint from "@eslint/js";
import stylistic from "@stylistic/eslint-plugin";
import jsdoc from "eslint-plugin-jsdoc";
import tsdoc from "eslint-plugin-tsdoc";
import tseslint from "typescript-eslint";

export default tseslint.config(
    eslint.configs.recommended,
    ...tseslint.configs.recommendedTypeChecked,
    jsdoc.configs["flat/contents-typescript"],
    jsdoc.configs["flat/logical-typescript"],
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
            "tjsdoc/text-escaping": "off",
            "no-case-declarations": "off",
            "jsdoc/check-tag-names": "off",
            "jsdoc/match-description": "off",
            "jsdoc/informative-docs": "off",
            "jsdoc/text-escaping": "off",
            "@typescript-eslint/triple-slash-reference": "off",
            "@typescript-eslint/restrict-template-expressions": "off",
            "@typescript-eslint/require-await": "warn",
            "@typescript-eslint/prefer-find": "warn",
            "@typescript-eslint/no-unused-vars": [
                "error",
                {
                    varsIgnorePattern: "^_",
                    caughtErrorsIgnorePattern: "^_",
                    argsIgnorePattern: "^_",
                },
            ],
            "@typescript-eslint/no-unsafe-return": "off",
            "@typescript-eslint/no-unsafe-member-access": "off",
            "@typescript-eslint/no-unsafe-assignment": "off",
            "@typescript-eslint/no-unsafe-argument": "off",
            "@typescript-eslint/no-unnecessary-condition": "warn",
            "@typescript-eslint/no-misused-promises": "off",
            "@typescript-eslint/no-floating-promises": "off",
            "@typescript-eslint/no-empty-object-type": "warn",
            "@stylistic/type-annotation-spacing": "warn",
            "@stylistic/space-infix-ops": "warn",
            "@stylistic/space-in-parens": "warn",
            "@stylistic/space-before-blocks": "warn",
            "@stylistic/quotes": "warn",
            "@stylistic/quote-props": "warn",
            "@stylistic/padded-blocks": "warn",
            "@stylistic/operator-linebreak": "warn",
            "@stylistic/object-curly-spacing": "warn",
            "@stylistic/no-trailing-spaces": "warn",
            "@stylistic/no-multiple-empty-lines": "warn",
            "@stylistic/no-multi-spaces": "warn",
            "@stylistic/multiline-ternary": "off",
            "@stylistic/max-statements-per-line": "warn",
            "@stylistic/keyword-spacing": "warn",
            "@stylistic/key-spacing": "warn",
            "@stylistic/indent-binary-ops": "off",
            "@stylistic/indent": "off",
            "@stylistic/eol-last": "warn",
            "@stylistic/comma-spacing": "warn",
            "@stylistic/comma-dangle": "warn",
            "@stylistic/brace-style": "warn",
            "@stylistic/arrow-spacing": "warn",
            "@stylistic/arrow-parens": "warn",
        },
    },
);
