const path = require('path');

module.exports = {
    mode: 'development',
    entry: {
        index: './src/index.js',
        tabulator: "./src/app-tabulator.js"
    },
    devtool: 'inline-source-map',
    optimization: {
        minimize: false
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [
                    'style-loader',
                    'css-loader',
                ]
            },
            {
                test: /\.scss$/,
                use: [
                    'sass-loader'
                ]
            }
        ]
    },
    output: {
        filename: '[name].bundle.js',
        path: path.resolve(__dirname, 'dist'),
        clean: true,
    }
};