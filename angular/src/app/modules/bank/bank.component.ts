import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { InputFilterDto } from '@shared/filter/filter.component';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { BankService } from './../../service/api/bank.service';
import { CreateEditBankComponent } from './create-edit-bank/create-edit-bank.component';

@Component({
  selector: 'app-bank',
  templateUrl: './bank.component.html',
  styleUrls: ['./bank.component.css']
})
export class BankComponent extends PagedListingComponentBase<any>{

  Directory_Bank_Create = PERMISSIONS_CONSTANT.Directory_Bank_Create;
  Directory_Bank_Delete = PERMISSIONS_CONSTANT.Directory_Bank_Delete;
  Directory_Bank_Edit = PERMISSIONS_CONSTANT.Directory_Bank_Edit;
  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'Name', comparisions: [0, 6, 7, 8], displayName: "filterDirectory.Name" },
    { propertyName: 'Code', comparisions: [0, 6, 7, 8], displayName: "filterDirectory.Code" },
  ];
  constructor(
    injector: Injector,
    private _bankService: BankService,
    private dialog: MatDialog,
  ) {
    super(injector);
  }

  banks: BankDto[] = [];

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._bankService
      .getAllPaging(request)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.banks = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }

  delete(bank: BankDto): void {
    abp.message.confirm(
      this.l("Delete bank '") + bank.name + "'?",
      '',
      (result: boolean) => {
        if (result) {
          this._bankService.delete(bank.id).subscribe(() => {
            abp.notify.success(this.l('Deleted bank ')+bank.name);
            this.refresh();
          });
        }
      }
    );
  }


  editBank(bank: BankDto): void {
    this.showDialogBanks(bank,"edit");
  }

  createBank(): void {
    let bank = {} as BankDto;
    this.showDialogBanks(bank,"create");
  }

  showDialogBanks(bank: BankDto,command:string): void {
    let item = { id: bank.id, name: bank.name, code: bank.code } as BankDto;
    const dialogRef = this.dialog.open(CreateEditBankComponent, {
      data: {item:item,
        command:command,},
     
      width: '500px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
     this.refresh();
    });
  }
}

export class BankDto {
  id: number;
  name: string;
  code: string;
}
