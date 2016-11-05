var path = require("path");
var webpack = require("webpack");
const autoprefixer = require('autoprefixer');
var ExtractTextPlugin = require('extract-text-webpack-plugin');

var cfg = {
  entry: {
    main: "./temp/main",
    renderer: "./temp/renderer"
  },
  output: {
    filename: "[name].js",
    path: "./app/js",
    libraryTarget: "commonjs2"
  },
  externals: {
    electron: true
  },
  target: "node",
  node: {
    __dirname: false,
    __filename: false
  },
  devtool: "source-map",
  module: {
    preLoaders: [{
      loader: "source-map-loader",
      exclude: /node_modules/,
      test: /\.js$/
    }],
    loaders: [
	  {
        test: /(\.scss|\.css)$/,
        loader: ExtractTextPlugin.extract('style', 'css?sourceMap&modules&importLoaders=1&localIdentName=[name]__[local]___[hash:base64:5]!postcss!sass')
      }
	]
  },
  postcss: [autoprefixer],
  sassLoader: {
    data: '@import "./app/css/_config.scss";',
    includePaths: [path.resolve(__dirname, './out')]
  },
  plugins: [
    new ExtractTextPlugin('bundle.css', { allChunks: true })
  ]
};
module.exports = cfg
