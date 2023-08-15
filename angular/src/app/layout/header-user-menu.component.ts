import { SocialAuthService } from 'angularx-social-login';
import { Component, ChangeDetectionStrategy } from '@angular/core';
import { AppAuthService } from '@shared/auth/app-auth.service';

@Component({
  selector: 'header-user-menu',
  templateUrl: './header-user-menu.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderUserMenuComponent {
  constructor(private _authService: AppAuthService, private googleAuthService: SocialAuthService) {}

  logout(): void {
    this._authService.logout();
    this.googleAuthService.signOut()
  }
}
