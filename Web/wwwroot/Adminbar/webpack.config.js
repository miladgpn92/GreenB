const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const CompressionPlugin = require("compression-webpack-plugin");
module.exports = {
    watch: true,

    mode: "production", //production | development
    entry: {
        dist: [
            "./wwwroot/Adminbar/assets/js/main.js"
        ],
        styles: [
            "material-symbols",
            "./wwwroot/Adminbar/assets/css/global/global.scss",
 

        ]
    },

    resolve: {
        extensions: [".mjs", ".js", ".scss", ".css", ".ts"],
        fallback: {
            fs: false,
            path: false,
        },
    },

    module: {
        rules: [
            {
                test: /\.css$/i,
                use: [MiniCssExtractPlugin.loader, "css-loader"],
            },
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    "css-loader",
                    "sass-loader",
                    {
                        loader: "postcss-loader",
                        options: {
                            postcssOptions: {
                                plugins: [require("tailwindcss"), require("autoprefixer")],
                            },
                        },
                    },
                ],
            },
            {
                test: /\.ts?$/,
                loader: "ts-loader",
                exclude: /node_modules|\.d\.ts$/,
            },
            {
                test: /\.d\.ts$/,
                loader: "ignore-loader",
            },
            {
                test: /\.(?:js|mjs|cjs)$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader",
                    options: {
                        presets: [["@babel/preset-env", { targets: "defaults" }]],
                    },
                },
            },
            {
                test: /\.(woff(2)?|ttf|eot)$/,
                type: "asset/resource",
                generator: {
                    filename: "./dist/assets/fonts/[name][hash][ext]",
                },
            },

            {
                test: /\.(svg|png|jpg|jpeg)$/,
                type: "asset/resource",
                generator: {
                    filename: "./dist/assets/images/[name][hash][ext]",
                },
            },
        ],
    },
    optimization: {
        minimizer: [new CssMinimizerPlugin(), new TerserPlugin()],
    },

    plugins: [
        new webpack.optimize.ModuleConcatenationPlugin({
            optimizationBailout: true,
        }),

        new MiniCssExtractPlugin({
            filename: "./dist/css/[name].css",
        }),
        new CompressionPlugin({
            algorithm: "gzip",
        }),
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery",
            jquer: "jquery",
            "window.jquery": "jquery",
            process: "process/browser.js",
        }),
    ],
    ignoreWarnings: [
        {
            module: /module2\.js\?[34]/, // A RegExp
        },
        {
            module: /[13]/,
            message: /homepage/,
        },
        /warning from compiler/,
        (warning) => true,
    ],
    output: {
        filename: "dist/[name].js",
        path: path.resolve(__dirname),
    },
};
