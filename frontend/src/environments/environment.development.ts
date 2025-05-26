export const environment = {
  production: false,
  apiUrl: 'http://localhost:8080',
  // apiUrl: 'https://travelwise-dev.up.railway.app',
  googleClientId: '',
  microsoftConfig: {
    clientId: '',
    authority: 'https://login.microsoftonline.com/85707f27-830a-4b92-aa8c-3830bfb6c6f5',
    redirectUri: 'http://localhost:4200',
    postLogoutRedirectUri: 'http://localhost:4200'
  }
};
