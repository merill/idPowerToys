const { createProxyMiddleware } = require('http-proxy-middleware');

const context = [
    "/powerpoint",
    "/weatherforecast",
];

module.exports = function (app) {
    const appProxy = createProxyMiddleware(context, {
        target: 'https://localhost:7172',
        secure: false
    });

    app.use(appProxy);
};
