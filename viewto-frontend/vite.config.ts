import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tsc from 'vite-plugin-tsc';

// https://vitejs.dev/config/
export default defineConfig({

    define:{ 
        "process.env": process.env,
    },

    plugins: [
        react({
            babel: {
                parserOpts: {
                    plugins: ['decorators-legacy', 'classProperties'],
                },
                plugins: [
                    ["@babel/plugin-proposal-decorators", { legacy: true }],
                    ["@babel/plugin-proposal-class-properties", { loose: true }],
                ],
            },
        }),
        tsc(),
    ],
})
