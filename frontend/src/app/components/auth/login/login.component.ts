import { AfterViewInit, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';
import { environment } from '../../../../environments/environment';

declare var google: any;
declare var msal: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
})
export class LoginComponent implements AfterViewInit {
  loginForm: FormGroup;
  errorMessage: string = '';
  private msalInstance: any;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  ngAfterViewInit() {
    // Initialize Google Sign-In
    if (typeof google !== 'undefined') {
      google.accounts.id.initialize({
        client_id: environment.googleClientId,
        callback: this.handleCredentialResponse.bind(this)
      });

      google.accounts.id.renderButton(
        document.getElementById('google-btn'),
        {
          theme: 'outline',
          size: 'large',
          width: '100%',
          text: 'continue_with'
        }
      );
    } else {
      console.error('Google Identity Services script not loaded');
    }

    // Initialize Microsoft Sign-In
    if (typeof msal !== 'undefined') {
      this.msalInstance = new msal.PublicClientApplication({
        auth: environment.microsoftConfig,
        cache: {
          cacheLocation: 'sessionStorage',
          storeAuthStateInCookie: false
        }
      });
    } else {
      console.error('MSAL script not loaded');
    }
  }

  handleCredentialResponse(response: any) {
    if (response.credential) {
      this.authService.googleLogin(response.credential).subscribe({
        next: () => {
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          console.error('Backend login error:', err);
          this.errorMessage = err.error?.message || 'Google login failed. Please try again.';
        }
      });
    }
  }

  async handleMicrosoftLogin() {
    try {
      const loginResponse = await this.msalInstance.loginPopup({
        scopes: ['user.read', 'email', 'profile'],
        prompt: 'select_account'
      });

      if (loginResponse) {
        const account = this.msalInstance.getAllAccounts()[0];
        const tokenResponse = await this.msalInstance.acquireTokenSilent({
          scopes: ['user.read', 'email', 'profile'],
          account: account
        });

        this.authService.microsoftLogin(tokenResponse.idToken).subscribe({
          next: () => {
            this.router.navigate(['/dashboard']);
          },
          error: (err) => {
            console.error('Microsoft login error:', err);
            this.errorMessage = err.error?.message || 'Microsoft login failed. Please try again.';
          }
        });
      }
    } catch (error) {
      console.error('Microsoft login error:', error);
      this.errorMessage = 'Microsoft login failed. Please try again.';
    }
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe({
        next: () => {
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          console.error('Login failed:', error);
          this.errorMessage = error.error?.message || 'Login failed. Please try again.';
        },
      });
    }
  }
}
