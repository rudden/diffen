var path = require('path')
var webpack = require('webpack')
var CleanWebpackPlugin = require('clean-webpack-plugin')
var BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin
var ExtractTextPlugin = require('extract-text-webpack-plugin')

const extractSass = new ExtractTextPlugin({ filename: "css/[name].css" })

module.exports = {
	entry: {
		'theme': './src/scripts/javascript/theme.js',
		'avatar': './src/scripts/javascript/avatar.js',
		
		'home': './src/scripts/pages/home',
		'forum': './src/scripts/pages/forum',
		'squad': './src/scripts/pages/squad',
		'profile': './src/scripts/pages/profile',
		'chronicle': './src/scripts/pages/chronicle',
		'poll': './src/scripts/pages/poll',
		'region': './src/scripts/pages/region',
		
		'style': './src/styles/app.scss'
	},
	output: {
		path: path.join(__dirname, './wwwroot/dist'),
		publicPath: '../',
		filename: 'js/[name].js'
	},
	resolve: {
		extensions: ['.ts', '.js', '.json', 'css'],
		modules: ['node_modules'],
	},
	module: {
		rules: [
			{
				test: /\.vue$/,
				loader: 'vue-loader'
			},
			{
				test: /\.ts$/,
				loader: 'ts-loader',
				options: {
					appendTsSuffixTo: [/\.vue$/],
				}
			},
			{
				test: /\.(scss|css)$/,
				use: extractSass.extract({
					use: [
						{
							loader: "css-loader"
						}, 
						{
							loader: "sass-loader"
						}
					],
					fallback: "style-loader"
				})
			},
			{
				test: /\.(png|jpe?g|gif|svg|ico)$/,
				loader: 'url-loader',
				options: {
					limit: 10000,
					name: 'img/[name].[ext]'
				},
				exclude: /fonts/
			},
			{
				test: /\.(woff|woff2|ttf|eot|svg)$/,
				loader: 'url-loader',
				options: {
					limit: 5000,
					name: 'fonts/[name].[ext]'
				},
				exclude: /img/
			}
		]
	},
	plugins: [
		new CleanWebpackPlugin(path.join(__dirname, 'wwwroot/dist'), { root: __dirname }),
		new webpack.ProvidePlugin({
			$: 'jquery',
			jquery: 'jquery',
			'window.jQuery': 'jquery',
			jQuery: 'jquery'
		}),
		extractSass
	]
}