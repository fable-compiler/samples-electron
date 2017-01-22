module.exports = {
  entry: {
    main: "./temp/src/main",
    renderer: "./temp/src/renderer"
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
    rules: [{
      enforce: "pre",
      loader: "source-map-loader",
      exclude: /node_modules/,
      test: /\.js$/
    }]
  }
};
