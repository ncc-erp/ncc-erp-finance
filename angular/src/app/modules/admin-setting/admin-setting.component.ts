import { AppComponentBase } from '@shared/app-component-base';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { AppConfigurationService } from './../../service/api/app-configuration.service';
import { Component, OnInit, Injector } from '@angular/core';
import { SessionServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppConsts } from '@shared/AppConsts';
import { forkJoin } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { ActivatedRoute } from '@angular/router';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-admin-setting',
  templateUrl: './admin-setting.component.html',
  styleUrls: ['./admin-setting.component.css']
})
export class AdminSettingComponent extends AppComponentBase implements OnInit {
  Admin_Configuration = PERMISSIONS_CONSTANT.Admin_Configuration;

  configuration = {} as ConfigurationDto
  googleToken: string = ""
  public isEditClientId: boolean = false
  public isEditSecretKey: boolean = false
  public isEditUserBot: boolean = false
  public isEditPassBot: boolean = false
  public isEditKomuUrl: boolean = false
  public isEditFinanceUrl: boolean = false
  public isEditGoogleKey: boolean = false
  public isEditProject: boolean = false
  public isEditLinkOutComing: boolean = false
  public isEditKomuChanel: boolean = false;
  public linkOutComing = {} as LinkOutcomingConfigurationDto;
  public isLoading: boolean = false;
  public canApplyMutltiCurrencyOutcome: boolean = false
  requestChiSetting = {} as RequestChiSettingDto
  public status: string;
  public allowChangeEntityInPeriodClosed: boolean = false
  public isEditRequestChiSetting: boolean = false
  public isEditCoTheSuaThongTinCuaKiCu: boolean = false
  public isEnableCrawlBTransactionNoti: boolean = false
  public hrmConfig = {} as internalToolConfig
  routeTitleFirstLevel = this.APP_CONSTANT.TitleBreadcrumbFirstLevel.admin;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.admin;
  routeTitleSecondLevel = this.APP_CONSTANT.TitleBreadcrumbSecondLevel.configuration;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.configuration;

  constructor(private settingService: AppConfigurationService, private sessionService: SessionServiceProxy, injector: Injector) {
    super(injector)
  }

  ngOnInit(): void {
    this.getSetting()
    this.getConfigLinkOutComing()
    this.getRequestChiSetting()
    this.getAllowChangeEntityInPeriodClosed()
    this.getEnableCrawlBTransactionNoti()
    this.getHRMConfig()
    this.updateBreadCrumb();
  }

  onRefreshCurrentPage(){
    this.ngOnInit();
  }

  updateBreadCrumb() {
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.routeTitleSecondLevel , url: this.routeUrlSecondLevel }
    ];
  }
  getSetting() {
    this.isLoading = true;
    this.settingService.getConfiguration().subscribe(data => {
      this.configuration = data.result;
      this.isLoading = false;
    }, () => this.isLoading = false);
  }
  getConfigLinkOutComing() {
    this.settingService.getCanLinkWithOutComingEnd().subscribe(response => {
      if (!response.success) return;
      this.linkOutComing.canLinkWithOutComingEnd = response.result;
    })
  }

  getEnableCrawlBTransactionNoti() {
    this.settingService.GetEnableCrawlBTransactionNoti().subscribe(response => {
      if (!response.success) return;
      this.isEnableCrawlBTransactionNoti = response.result;
    })
  }

  setEnableCrawlBTransactionNoti() {
    this.settingService.SetEnableCrawlBTransactionNoti(this.isEnableCrawlBTransactionNoti).subscribe(response => {
      if (!response.success) return;
      this.getEnableCrawlBTransactionNoti()
      abp.notify.success("update successful")
    })
  }

  setConfigLinkOutComing() {
    this.settingService.setCanLinkWithOutComingEnd(String(this.linkOutComing.canLinkWithOutComingEnd)).subscribe(response => {
      if (!response.success) return;
      this.getConfigLinkOutComing()
      abp.notify.success("update successful")
    })
  }

  getRequestChiSetting(){
    if(!this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_ViewRequestChiSetting)) return;
    this.settingService.GetRequestChiSetting()
    .subscribe(response => {
      if(!response.success) return;
      this.requestChiSetting.canApplyMutltiCurrencyOutcome = response.result.canApplyMutltiCurrencyOutcome == "true";
      this.requestChiSetting.maLoaiChiBangLuong = response.result.maLoaiChiBangLuong;
      this.settingService.CheckStatusOfMaLoaiChi(this.requestChiSetting.maLoaiChiBangLuong)
      .subscribe(rs => {
        this.status = rs.result;
      })
    })
  }



  getApplyToMultiCurrencyOutcome() {
    this.settingService.GetApplyToMultiCurrencyOutcome().subscribe(response => {
      if (!response.success) return;
      this.canApplyMutltiCurrencyOutcome = response.result == "true";
    })
  }
  setApplyToMultiCurrencyOutcome() {
    this.settingService.SetApplyToMultiCurrencyOutcome(String(this.canApplyMutltiCurrencyOutcome)).subscribe(response => {
      if (!response.success) return;
      AppConsts.CONFIG_CURRENCY_OUTCOMINGENTRY = this.canApplyMutltiCurrencyOutcome;
      this.getApplyToMultiCurrencyOutcome()
      abp.notify.success("update successful")
    })
  }
  getAllowChangeEntityInPeriodClosed() {
    this.settingService.getAllowChangeEntityInPeriodClosed().subscribe(response => {
      if (!response.success) return;
      this.allowChangeEntityInPeriodClosed = response.result;
    })
  }
  setAllowChangeEntityInPeriodClosed() {
    this.settingService.setAllowChangeEntityInPeriodClosed(this.allowChangeEntityInPeriodClosed).subscribe(response => {
      if (!response.success) return;
      this.getAllowChangeEntityInPeriodClosed()
      abp.notify.success("update successful")
    })
  }




  public changeClientAppId() {
    let input = {
      clientAppId: this.configuration.clientAppId
    };
    this.isLoading = true;
    this.settingService.ChangeClientAppId(input).subscribe((rs) => {
      if (rs) {
        abp.notify.success("Change client app id successful");
        this.getSetting();
        this.isLoading = false;
      }
    }, () => this.isLoading = false);
  }

  public changeNotifyKomuChannel() {
    let input = {
      notifyToChannel: this.configuration.notifyToChannel
    };
    let api1 = this.settingService.ChangeNotifyKomuChannel(input);
    let api2 = this.settingService.SetEnableCrawlBTransactionNoti(this.isEnableCrawlBTransactionNoti)
    this.isLoading = true;

    forkJoin(api1,
      api2)
      .subscribe(() => {
        abp.notify.success("Change notify komu channel succesful");
        this.isLoading = false;
        this.getEnableCrawlBTransactionNoti()
        this.getSetting()
      },
      () => this.isLoading = false)
  }

  public updateRequestChiSetting()
  {
    this.isLoading = true;
    this.settingService.UpdateRequestChiSetting(this.requestChiSetting).subscribe((rs) => {
      abp.notify.success("Update Request Chi Setting Sucessful");
      this.getRequestChiSetting();
      this.isLoading = false;
    }, () => this.isLoading = false);
  }

  public changeFinanceSecretKey() {
    let input = {
      SecretKey: this.configuration.secretKey
    };
    this.isLoading = true;
    this.settingService.ChangeFinanceSecretKey(input).subscribe((rs) => {
      if (rs) {
        abp.notify.success("Change secret key successful");
        this.getSetting();
        this.isLoading = false;
      }
    }, () => this.isLoading = false);
  }


  getHRMConfig(){
    this.settingService.getHRMConfig().subscribe(rs => {
      this.hrmConfig = rs.result
    })
  }

  isShowEditKomuSettingBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_EditKomuSetting);
  }
  isShowEditGoogleSettingBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_EditGoogleSetting);
  }
  isShowEditSecretKeyBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_EditSecretKey);
  }
  isShowEditLinkToRequestChiDone() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_EditLinkToRequestChiDaHoanThanh);
  }
  isShowEditRequestChiSettingBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_EditRequestChiSetting);
  }
  isViewRequestChiSetting(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_ViewRequestChiSetting);
  }
  isShowEditAllowChangeEntityInPeriodClosedBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_EditCoTheSuaThongTinCuaKiCu);
  }

}
export class ConfigurationDto {
  clientAppId: string;
  secretKey: string;
  notifyToChannel: string;
}
export class RequestChiSettingDto {
  canApplyMutltiCurrencyOutcome: boolean;
  maLoaiChiBangLuong: string;
}
export class LinkOutcomingConfigurationDto {
  canLinkWithOutComingEnd: boolean
}
export interface internalToolConfig{
  baseAddress:string,
  securityCode:string
}

