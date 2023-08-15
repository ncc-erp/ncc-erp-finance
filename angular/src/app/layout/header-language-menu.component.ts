import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
  Injector
} from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import {
  UserServiceProxy,
  ChangeUserLanguageDto
} from '@shared/service-proxies/service-proxies';
import { filter as _filter } from 'lodash-es';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'header-language-menu',
  templateUrl: './header-language-menu.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderLanguageMenuComponent extends AppComponentBase
  implements OnInit {
  languages: abp.localization.ILanguageInfo[];
  currentLanguage: abp.localization.ILanguageInfo;
  isEnglish = false;
  constructor(injector: Injector, private _userService: UserServiceProxy, private translate: TranslateService) {
    super(injector);
    translate.setDefaultLang('vn');
    translate.use('vn');
  }

  ngOnInit() {
    // this.languages = _filter(
    //   this.localization.languages,
    //   (l) => !l.isDisabled
    // );
    // this.currentLanguage = this.localization.currentLanguage;
  }
  switchLanguage(language: string) {
    this.translate.use(language);
    if ( language === 'en') {
      this.isEnglish = true;
    } else {
      this.isEnglish = false;
    }
  }

  changeLanguage(languageName: string): void {
    const input = new ChangeUserLanguageDto();
    input.languageName = languageName;

    this._userService.changeLanguage(input).subscribe(() => {
      abp.utils.setCookieValue(
        'Abp.Localization.CultureName',
        languageName,
        new Date(new Date().getTime() + 5 * 365 * 86400000), // 5 year
        abp.appPath
      );

      window.location.reload();
    });
  }
}
