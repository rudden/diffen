var path = require('path')
var webpack = require('webpack')
var CleanWebpackPlugin = require('clean-webpack-plugin')
var BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin
var ExtractTextPlugin = require('extract-text-webpack-plugin')

const extractSass = new ExtractTextPlugin({ filename: "css/[name].css" })

module.exports = {
	entry: {
		'bundles': './src/scripts/vendor.ts',
		'forum': './src/scripts/pages/forum',
		'squad': './src/scripts/pages/squad',
		'profile': './src/scripts/pages/profile',
		'style': './src/styles/app.scss'
	},
	output: {
		path: path.join(__dirname, './dist'),
		publicPath: '/dist/',
		filename: 'script/[name].js'
	},
	resolve: {
		extensions: ['.ts', '.js', '.json', 'scss'],
		modules: ['node_modules']
	},
	module: {
		rules: [
			{
				test: /\.vue$/,
				loader: 'vue-loader'
			},
			{
				test: /\.tsx$/,
				loader: 'ts-loader',
				options: {
					appendTsSuffixTo: [/\.vue$/],
				}
			},
			{
				test: /\.scss$/,
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
				test: /\.(png|jpg|gif|svg)$/,
				loader: 'file-loader',
				options: {
					name: '[name].[ext]?[hash]'
				}
			}
		]
	},
	plugins: [
		new CleanWebpackPlugin(path.join(__dirname, 'dist'), { root: __dirname }),
		new webpack.ProvidePlugin({
			$: 'jquery',
			jquery: 'jquery',
			'window.jQuery': 'jquery',
			jQuery: 'jquery'
		}),
		extractSass
	]
}