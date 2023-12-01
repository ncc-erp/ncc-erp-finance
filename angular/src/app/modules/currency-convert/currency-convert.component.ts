import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Route } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { CurrencyService } from '@app/service/api/currency.service';
import { Utils } from '@app/service/helpers/utils';
import { AppConsts } from '@shared/AppConsts';

import { FilterDto, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import {CurrencyConvertService} from '../../service/api/currency-convert.service';
import { EComparisor, PAGE_SIZE_OPTIONS } from '../revenue-managed/revenue-managed.component';
import { CreateEditCurrencyConvertComponent } from './create-edit-currency-convert/create-edit-currency-convert.component';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-currency-convert',
  templateUrl: './currency-convert.component.html',
  styleUrls: ['./currency-convert.component.css']
})
export class CurrencyConvertComponent extends PagedListingComponentBase<CurrencyConvertComponent> implements OnInit {


  constructor(injector: Injector,
    private currencyConvertService: CurrencyConvertService,
    private _currencyService: CurrencyService,
    private route: ActivatedRoute,
    public dialog: MatDialog,
    private translate: TranslateService) {
    super(injector);
    this.applyUrlFilters();
  }

  public listCurrencyConvert:CurrencyConvertDto[] = [];
  public readonly LIST_PAGE_SIZE_OPTIONS = PAGE_SIZE_OPTIONS;
  public listCurrencies: CurrencyDto[] = [];
  searchDetail = {
    currencyId: AppConsts.VALUE_OPTIONS_ALL
  };
  public listMonths:Number[] = [];
  public listYears:Number[] = [];
  public selectedMonth: number | string;
  public selectedYear: number | string;
  public sortProperty: string = "";
  public sortDirection: number = null
  public sortDirectionEnum = SortDirectionEnum;
  public filterItems: FilterDto[] = [];
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu3;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.currencyConvert;
  queryParams;

  async ngOnInit(){
    await this.getYearOfCurrencyConvert();
    this.getAllCurrency();
    this.getMonthOfCurrencyConvert();
    if(!this.filterItems.length) {
      this.sortDirection = this.sortDirectionEnum.Descending;
      this.sortProperty = 'dateAt'
    }
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.isTableLoading = true;
    const payload = this.getRequestParams(request);
    this.currencyConvertService.getAllPaging(payload)
    .pipe(finalize(() => {
      finishedCallback();
    }))
    .subscribe((rs)=>{
      this.listCurrencyConvert = rs.result.items;
      this.showPaging(rs.result, pageNumber);
      this.isTableLoading = false;
    }, ()=> this.isTableLoading = false)
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
    this.translate.get("menu3.m3_child8").subscribe((res: string) => {
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
  protected delete(entity: CurrencyConvertComponent): void {
    throw new Error('Method not implemented.');
  }



  onCreateCurrencyConvert(){
    this.showDialog(null, 'Thêm mới tỉ giá');
  }
  editCurrencyConvert(data: CurrencyConvertDto){
    this.showDialog(data, 'Sửa đổi tỉ giá');
  }
  showDialog(data, title){
    let input = {
      title: title,
      currencyConvert: data
    }
    let dl = this.dialog.open(CreateEditCurrencyConvertComponent,{
      data: input,
      width: "700px"
    })
    dl.afterClosed().subscribe((rs)=>{
      if(rs){
        this.getFirstPage();
        this.getMonthOfCurrencyConvert();
      }
    })

  }

  public onDelete(cc: CurrencyConvertDto){
    abp.message.confirm(
      this.l("Bạn có muốn xóa tỉ giá của'") + cc.currencyName + "'?",
      '',
      (result: boolean) => {
        if (result) {
          this.currencyConvertService.delete(cc.id).subscribe(() => {
            abp.notify.success(this.l('Bạn đã xóa tỉ giá thành công'));
            this.refresh();
          });
        }
      }
    );
  }

  public getAllCurrency(){
    this._currencyService.getAllCurrencyForDropdown().subscribe((rs)=>{
      this.listCurrencies = rs.result;
    })
  }


  private getRequestParams(request: PagedRequestDto):InputGetCurrencyConvert {
    const payload = { ...request } as InputGetCurrencyConvert;
    this.filterItems = [];

    for (const property in this.searchDetail) {
      if (!Utils.isSelectedOptionsAll(this.searchDetail[property])) {
        const filterObj = {
          propertyName: property,
          value: this.searchDetail[property],
          comparision: EComparisor.EQUAL,
        }
        this.filterItems.push(filterObj);
      }
    }
    payload.filterItems = this.filterItems;
    if(this.selectedMonth != 0){
      payload.month = Number(this.selectedMonth);
    }
    if(this.selectedYear != 0){
      payload.year = Number(this.selectedYear);
    }

    if (this.sortProperty) {
      payload.sort = this.sortProperty;
      payload.sortDirection = this.sortDirection;
    }
    return payload;
  }


  public async getYearOfCurrencyConvert(){
    await this.currencyConvertService.getYearOfCurrencyConvert().then((rs)=>{
      this.listYears = rs.result;
      if(this.selectedYear && this.listYears.includes(Number(this.selectedYear))) {
        this.refresh();
        return;
      }
      this.checkListYearHasCurrentYear();
    })
  }
  public getMonthOfCurrencyConvert(){
  this.currencyConvertService.getMonthOfCurrencyConvert().subscribe((rs)=>{
      this.listMonths = rs.result;
    })
  }
  public onSortChange(property: string) {
    if (this.sortProperty != property) {
        this.sortDirection = null
    }
    if (property) {
        switch (this.sortDirection) {
            case null: {
                this.sortDirection = this.sortDirectionEnum.Ascending
                this.sortProperty = property
                break;
            }
            case this.sortDirectionEnum.Ascending: {
                this.sortDirection = this.sortDirectionEnum.Descending
                this.sortProperty = property
                break;
            }
            case this.sortDirectionEnum.Descending: {
                this.sortDirection = null
                this.sortProperty = ""
                break;
            }
        }
    }
    this.refresh()
}
checkListYearHasCurrentYear(){
  let currentYear = new Date().getFullYear();
  if(this.listYears.includes(currentYear)){
    this.selectedYear = currentYear;
  }else{
    this.selectedYear = AppConsts.VALUE_OPTIONS_ALL;
  }
  this.refresh();
}
isShowCreateBtn(){
  return this.permission.isGranted(PERMISSIONS_CONSTANT.Directory_CurrencyConvert_Create)
}
isShowEditBtn(){
  return this.permission.isGranted(PERMISSIONS_CONSTANT.Directory_CurrencyConvert_Edit)
}
isShowDeleteBtn(){
  return this.permission.isGranted(PERMISSIONS_CONSTANT.Directory_CurrencyConvert_Delete)
}
applyUrlFilters(){
  var querySnapshot = this.route.snapshot.queryParams
    this.selectedYear = querySnapshot['year'] ? Utils.toNumber(querySnapshot['year']) : AppConsts.VALUE_OPTIONS_ALL;
    this.selectedMonth = querySnapshot['month'] ? Utils.toNumber(querySnapshot['month']) : AppConsts.VALUE_OPTIONS_ALL;
}


}
export class CurrencyConvertDto{
  id: number;
  currencyName: string;
  currencyId: number;
  value: number;
  valueFormat: string;
  dateAt: string;
}
export class CurrencyDto {
  id: number;
  name: string;
  code: string;
  maxITF: number;
  defaultBankAccountId: number;
}

export class InputGetCurrencyConvert extends PagedRequestDto{
  month: number;
  year: number;
}
export enum SortDirectionEnum {
  Ascending = 0,
  Descending = 1
}
