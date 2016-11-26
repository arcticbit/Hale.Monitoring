const webpack = require('webpack');
const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
    context: __dirname,
    entry: [
        'babel-polyfill',
        './src/app.js'
    ],
    output: {
        path: __dirname + '/dist',
        filename: 'app.js'
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: './index.template.html'
        }),
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery"
        })
    ],
    module: {
        loaders: [
            { test: /\.html$/, loader: "ngtemplate!html", exclude: /index\.template\.html/ },
            { test: /\.css$/,  loader: "style!css" },
            { test: /\.scss$/, loader: "style!css!sass" },
            { test: /\.js$/,   loader: "babel", exclude: /node_modules/, query: { presets: ['es2015']}},
            {
              test: /\.(ttf|eot|svg|woff|woff2)(\?v=[0-9]\.[0-9]\.[0-9])?$/,
              loader: "file-loader"
            }
        ]
    }
};