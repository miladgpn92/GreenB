const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const CompressionPlugin = require("compression-webpack-plugin");
const glob = require("glob");
const tailwindcss = require("tailwindcss");
const autoprefixer = require("autoprefixer");
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;


module.exports = {
   watch: true,

  mode: "development", //production | development
  entry: {
    bundle: [
      "./wwwroot/DTCMS/assets/js/main.js",
      "./wwwroot/Shared/libs/jquery/dist/jquery.min.js",
      "./wwwroot/Shared/libs/jquery-validation/dist/jquery.validate.min.js",
      "./wwwroot/Shared/libs/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
    ],
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
        use: ["style-loader", MiniCssExtractPlugin.loader, "css-loader"],
      },
      //   {
      //     loader: "imports-loader",
      //     test: require.resolve("./wwwroot/Admin/assets/ts/components/global/index.ts"),

      //     options: {
      //       imports: [
      //         {
      //           syntax: "default",
      //           moduleName: "jquery",
      //           name: "$",
      //         },
      //         {
      //           syntax: "default",
      //           moduleName: "jquery-validation",
      //           name: "validate",
      //         },
      //       ],
      //     },
      //   },

      {
        test: /\.scss$/,
        use: [
          "style-loader",
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
          filename: "./bundle/assets/fonts/[name][hash][ext]",
        },
      },

      {
        test: /\.(svg|png|jpg|jpeg)$/,
        type: "asset/resource",
        generator: {
          filename: "./bundle/assets/images/[name][hash][ext]",
        },
      },
    ],
  },
  optimization: {
    minimizer: [
      new CssMinimizerPlugin(),
      new TerserPlugin({
        parallel: true,
        terserOptions: {
          format: {
            comments: false,
          },
        },
        extractComments: false,
      }),
    ],
  },

    plugins: [
    
    new webpack.optimize.ModuleConcatenationPlugin({
      optimizationBailout: true,
    }),

    new MiniCssExtractPlugin({
      filename: "bundle/css/[name].css",
    }),
    new CompressionPlugin({
      algorithm: "gzip",
    }),
    new webpack.ProvidePlugin({
      $: "jquery",
      jQuery: "jquery",
      jquer: "jquery",
      "window.jquery": "jquery",
      "window.moment": "moment-jalaali",
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
    filename: "bundle/[name].js",
    path: path.resolve(__dirname, "wwwroot/DTCMS"),
  },
  devtool: false,
};
