export const msalConfig = {
    auth: {
        clientId: "afe793df-19e0-455a-8403-2e863379bfaa",
        authority: "https://login.microsoftonline.com/common", // This is a URL (e.g. https://login.microsoftonline.com/{your tenant ID})
        redirectUri: "http://localhost:3000/",
    },
    cache: {
        cacheLocation: "sessionStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
    }
};

// Add scopes here for ID token to be used at Microsoft identity platform endpoints.
export const loginRequest = {
    scopes: ["User.Read"]
};

// Add the endpoints here for Microsoft Graph API services you'd like to use.
export const graphConfig = {
    graphMeEndpoint: "https://graph.microsoft.com/v1.0/me"
};

export const apiConfig = {
    powerPointEndPoint: "https://idpowertoys.azurewebsites.net"
}