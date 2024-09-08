/**
 * @name build.js
 * @description Creates environment config file.
 */

const fs = require('fs');

async function run() {

    console.log('Copying authConfig file based on environment in idp_env_name variable');
    const envName = process.env.idp_env_name;
    const authConfigFile = `./src/authConfig.js`;
    const authConfigDevFile = `./buildscript/authConfig.ci.js`;
    const authConfigProdFile = `./buildscript/authConfig.prod.js`;

    if (envName === 'ci') {
        fs.copyFileSync(authConfigDevFile, authConfigFile);
    } else if (envName === 'prod') {
        fs.copyFileSync(authConfigProdFile, authConfigFile);
    } else {
        console.warn('Environment name not found assuming local dev.');
        return;
    }
}

run();
