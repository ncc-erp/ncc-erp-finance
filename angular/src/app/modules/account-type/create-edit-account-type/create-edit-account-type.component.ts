import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { catchError } from 'rxjs/operators';
import { AccountTypeService} from '../../../service/api/account-type.service';
import { AccountTypeDto} from '../account-type.component';
@Component({
  selector: 'app-create-edit-account-type',
  templateUrl: './create-edit-account-type.component.html',
  styleUrls: ['./create-edit-account-type.component.css']
})
export class CreateEditAccountTypeComponent extends AppComponentBase implements OnInit {
  isDisable: boolean = false;
  accountType = {} as AccountTypeDto;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private accountTypeService : AccountTypeService,
    injector: Injector,
    private router: Router,
    public dialogRef: MatDialogRef<CreateEditAccountTypeComponent>,

  ) { super(injector); }
  isEdit: boolean;
  ngOnInit(): void {
    this.accountType = this.data.item;
    this.isEdit = this.data.item.id === undefined ? false : true;
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this.accountTypeService.create(this.accountType).pipe(catchError(this.accountTypeService.handleError)).subscribe((res) => {
        abp.notify.success("created account type successfully");
        this.reloadComponent()
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
    else {
      this.accountTypeService.update(this.accountType).pipe(catchError(this.accountTypeService.handleError)).subscribe((res) => {
        abp.notify.success("edited account type successfully");
        this.reloadComponent()
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
  }
  reloadComponent() {
    this.router.navigateByUrl('', { skipLocationChange: true }).then(() => {
      this.router.navigate(['/app/accountType']);
    });
  }

}
