import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { CurrencyService } from './../../service/api/currency.service';
import { CreateCurrencyComponent } from './create-currency/create-currency.component';
import { EditCurrencyComponent } from './edit-currency/edit-currency.component';
import { PERMISSIONS_CONSTANT } from '../../../app/constant/permission.constant';
import { InputFilterDto } from 'shared/filter/filter.component';
import { finalize } from 'rxjs/operators';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';
import { SessionServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppConsts } from '@shared/AppConsts';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-currency',
  templateUrl: './currency.component.html',
  styleUrls: ['./currency.component.css']
})
export class CurrencyComponent extends PagedListingComponentBase<any> implements OnInit {

  Directory_Currency_Create = PERMISSIONS_CONSTANT.Directory_Currency_Create;
  Directory_Currency_Delete = PERMISSIONS_CONSTANT.Directory_Currency_Delete;
  Directory_Currency_Edit = PERMISSIONS_CONSTANT.Directory_Currency_Edit;
  Directory_Currency_ChangeDefaultCurrency = PERMISSIONS_CONSTANT.Directory_Currency_ChangeDefaultCurrency;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.directory;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.currencies;
  queryParams;

  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'Name', comparisions: [0, 6, 7, 8], displayName: "filterDirectory.Name" },
    { propertyName: 'Code', comparisions: [0, 6, 7, 8], displayName: "filterDirectory.Code" },
    { propertyName: 'Value', comparisions: [0, 1, 2, 3, 4, 5], displayName: "filterDirectory.Value" }
  ];

  constructor(
    private route: ActivatedRoute,
    injector: Injector,
    private _currencyService: CurrencyService,
    private _modalService: BsModalService,
    public _utilities: UtilitiesService,
   private sessionService: SessionServiceProxy,
   private translate: TranslateService
  ) {
    super(injector);

  }
  currencies = [] as CurrencyDto[];
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._currencyService
      .getAllPaging(request)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.currencies = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
    this.translate.onLangChange.subscribe(() => {
      this.onLangChange();
    });
    this.route.queryParams.subscribe(params => {
      this.queryParams = new HttpParams({ fromObject: params });
      this.onLangChange();
    });
  }
  
  onLangChange(){
    this.translate.get("menu.menu3").subscribe((res: string) => {
      this.routeTitleFirstLevel = res;
      this.updateBreadCrumb();
    });
    this.translate.get("menu3.m3_child1").subscribe((res: string) => {
      this.title = res;
      this.updateBreadCrumb();
    });
  }

  updateBreadCrumb() {
    let queryParamsString = this.queryParams.toString();
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.title , url: this.routeUrlSecondLevel + (queryParamsString ? '?' + queryParamsString : '')}
    ];
  }
  delete(currency: CurrencyDto): void {
    abp.message.confirm(
      this.l("Delete currency '") + currency.name + "'?",
      '',
      (result: boolean) => {
        if (result) {
          this._currencyService.delete(currency.id).subscribe(() => {
            abp.notify.success(this.l('Deleted currency successfully'));
            this.refresh();
          });
        }
      }
    );
  }
  createCurrency(): void {
    this.showCreateOrEditCurrencyDialog();
  }
  editCurrency(currency: CurrencyDto): void {
    this.showCreateOrEditCurrencyDialog(currency.id);
  }

  showCreateOrEditCurrencyDialog(id?: number): void {
    let createOrEditCurrencyDialog: BsModalRef;
    if (!id) {
      createOrEditCurrencyDialog = this._modalService.show(
        CreateCurrencyComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditCurrencyDialog = this._modalService.show(
        EditCurrencyComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditCurrencyDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
  editDefaultCurrency(data: CurrencyDto) {
    if (this.permission.isGranted(this.Directory_Currency_ChangeDefaultCurrency)) {
      abp.message.confirm(
        "Do you want to change default to currency?",
        "",
        (Success) => {
          if (!Success) return;
          this.processing = true;
          data.isCurrencyDefault = !data.isCurrencyDefault;
          this._currencyService.defaultCurrency(data)
            .subscribe(response => {
              if (!response.success) return;
              abp.notify.success("Set Default To Currency Success");
              this.refresh();
              this.processing = false;
              this.getCurrentLogin();
            })
        }); 
    }
  }
  getCurrentLogin() {
    this.sessionService.getCurrentLoginInformations().subscribe((rs)=>{
      AppConsts.DEFAULT_CURRENCY_CODE = rs.defaultCurrencyCode;
      AppConsts.DEFAULT_CURRENCY_ID = rs.defaultCurrencyId;

    });
  }
}

export class CurrencyConvertDto {
  id: number;
  name: string;
  code: string;
  value: number;
  maxITF: number;
  defaultBankAccountId: number;
  defaultBankAccountIdWhenSell: number;
  defaultFromBankAccountIdWhenBuy: number;
  defaultToBankAccountIdWhenBuy: number;
  isCurrencyDefault: boolean;
}

export class CurrencyDto{
  id: number;
  name: string;
  code: string;
  value: number;
  maxITF: number;
  defaultBankAccountId: number;
  defaultBankAccountName: string;
  defaultBankAccountIdWhenSell: number;
  defaultBankAccountNameWhenSell: string;
  defaultToBankAccountNameWhenBuy: string;
  defaultFromBankAccountIdWhenBuy: number;
  defaultFromBankAccountNameWhenBuy: string;
  defaultToBankAccountIdWhenBuy: number;
  isCurrencyDefault: boolean;
}
