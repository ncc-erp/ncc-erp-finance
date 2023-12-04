import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { MatDialog } from '@angular/material/dialog';
import { InputFilterDto } from './../../../shared/filter/filter.component';
import { finalize } from 'rxjs/operators';
import { PagedRequestDto } from './../../../shared/paged-listing-component-base';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import { SupplierService } from '@app/service/api/supplier.service';
import { CreateEditSupplierComponent } from './create-edit-supplier/create-edit-supplier.component';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-supplier-list',
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.css']
})
export class SupplierListComponent extends PagedListingComponentBase<SupplierListComponent> implements OnInit {
  protected delete(entity: SupplierListComponent): void {
    // throw new Error('Method not implemented.');
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.supplierService.getAllPaging(request).pipe(finalize(() => {
      finishedCallback();
    })).subscribe(data => {
      this.supplierList = data.result.items;
      this.showPaging(data.result, pageNumber);
    })
  }

  Directory_Supplier_Create = PERMISSIONS_CONSTANT.Directory_Supplier_Create;
  Directory_Supplier_Update = PERMISSIONS_CONSTANT.Directory_Supplier_Update;
  Directory_Supplier_Delete = PERMISSIONS_CONSTANT.Directory_Supplier_Delete;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.directory;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.supplierList;
  queryParams;



  supplierList: SupplierDto[] = []
  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'name', comparisions: [0, 6, 7, 8], displayName: "filterSupplierList.Name" },
    { propertyName: 'address', comparisions: [0, 6, 7, 8], displayName: "filterSupplierList.Address" },
    { propertyName: 'phoneNumber', comparisions: [0, 6, 7, 8], displayName: "filterSupplierList.SupplierPhone" },
    { propertyName: 'contactPersonPhone', comparisions: [0, 6, 7, 8], displayName: "filterSupplierList.ContactPhone" },
    { propertyName: 'contactPersonName', comparisions: [0, 6, 7, 8], displayName: "filterSupplierList.ContactPerson" },
    { propertyName: 'taxNumber', comparisions: [0, 1, 2, 3, 4, 5], displayName: "filterSupplierList.Taxnumber" }
  ];
  constructor(
    private route: ActivatedRoute,
    private dialog: MatDialog,
    injector: Injector,
    private supplierService: SupplierService,
    private translate: TranslateService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.refresh()
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
    this.translate.get("menu3.m3_child7").subscribe((res: string) => {
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
  editSupplier(supplier: SupplierDto): void {
    this.showDialogSupplier(supplier,"edit");
  }
  createSupplier(): void {
    let supplier = {} as SupplierDto;
    this.showDialogSupplier(supplier,"create");
  }
  deleteSupplier(supplier: SupplierDto): void {
    abp.message.confirm(
      this.l("Delete supplier '") + supplier.name + "'?",
      '',
      (result: boolean) => {
        if (result) {
          this.supplierService.delete(supplier.id).subscribe(() => {
            abp.notify.success(this.l('Deleted supplier '+supplier.name));
            this.refresh();
          });
        }
      }
    );
  }
  showDialogSupplier(supplier: SupplierDto,command:string): void {
    let item = {
      id: supplier.id, name: supplier.name, phoneNumber: supplier.phoneNumber, address: supplier.address,
      contactPersonName: supplier.contactPersonName, contactPersonPhone: supplier.contactPersonPhone, taxNumber: supplier.taxNumber
    } as SupplierDto;
    const dialogRef = this.dialog.open(CreateEditSupplierComponent, {
      data: {
        item:item,
        command:command
      },
      width: '680px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(() => {
      this.refresh()
    });
  }

}
export class SupplierDto {
  id: number;
  name: string;
  phoneNumber: string;
  address: string;
  contactPersonName: string;
  contactPersonPhone: string;
  taxNumber: string;
}