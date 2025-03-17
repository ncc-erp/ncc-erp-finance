import { LoginService } from './login.service';
import { Component, Injector } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { GoogleLoginProvider, SocialAuthService, SocialUser } from 'angularx-social-login';
import { Oauth2Mezon,AppConsts } from './../../shared/AppConsts';
import { ActivatedRoute, Router } from '@angular/router';
@Component({
  templateUrl: './login.component.html',
  animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase {
  submitting = false;
  user: SocialUser
  tenancyName: string
  loggedIn: boolean;
  isLoginMezon = AppConsts.enableLoginMezon;
  isLoginGoogle = AppConsts.enableLoginGoogle;
  isEnableNormalLogin = AppConsts.enableNormalLogin;
  constructor(
    injector: Injector,
    public authService: AppAuthService,
    private _sessionService: AbpSessionService,
    private googleAuthService: SocialAuthService,
    private loginService: LoginService,
    private route: ActivatedRoute
  ) {
    super(injector);
  }
  ngOnInit(): void {
    // this.googleAuthService.authState.subscribe((user) => {
    //   this.user = user;
    //   this.loggedIn = (user != null);
    //   if (this.loggedIn) {
    //     this.loginService.authenticateGoogle(this.user.idToken);
    //  }
    // });

    this.route.queryParams.subscribe(params => {
      const authorizationCode = params['code'];
      if(authorizationCode != null ){
        this.loginService.authenticateMezon(authorizationCode);
      }
    })
  }
  get multiTenancySideIsTeanant(): boolean {
    return this._sessionService.tenantId > 0;
  }

  get isSelfRegistrationAllowed(): boolean {
    if (!this._sessionService.tenantId) {
      return false;
    }

    return true;
  }

  login(): void {
    this.submitting = true;
    this.authService.authenticate(() => (this.submitting = false));
  }
  signInWithGoogle() {
    this.googleAuthService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }
  signInWithMezon(){
    const OAUTH2_AUTHORIZE_URL = Oauth2Mezon.OAUTH2_AUTHORIZE_URL;
    const CLIENT_ID = AppConsts.mezonClientId;
    const REDIRECT_URI = AppConsts.appBaseUrl+"/account/login";
     const RESPONSE_TYPE = 'code';
     const SCOPE = 'openid+offline';
     const STATE = 'hkjadkjashdkjsah'; 

    const authUrl = `${OAUTH2_AUTHORIZE_URL}?client_id=${CLIENT_ID}&redirect_uri=${REDIRECT_URI}&response_type=${RESPONSE_TYPE}&scope=${SCOPE}&state=${STATE}`;
		return (window.location.href = authUrl);
  }
}
