const path = require('path');
const webpack = require('webpack');

module.exports = {
    entry: './wwwroot/js/tests/keyManagerTests.js',
    output: {
        path: path.resolve(__dirname, 'wwwroot/dist'),
        filename: 'bundle.js'
    }

};