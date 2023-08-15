import { cloneOutcomingEntryDto, OutcomingEntryDto } from '@app/modules/expenditure-request/expenditure-request.component';
import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { CurrencyService } from './../../../service/api/currency.service';
import { forkJoin } from 'rxjs';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-clone-request',
  templateUrl: './clone-request.component.html',
  styleUrls: ['./clone-request.component.css']
})
export class CloneRequestComponent implements OnInit {
  public newOutcomingEntry = {} as cloneOutcomingEntryDto
  public currencyList: any
  public isShowMoney: boolean = false
  public isLoading: boolean = false

  constructor(
    public dialogRef: MatDialogRef<CloneRequestComponent>,
    private router: Router,
    private requestService: ExpenditureRequestService,
    private currencyService: CurrencyService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
    this.newOutcomingEntry = {
      name: this.data.name,
      value: this.data.value,
      currencyId: this.data.currencyId,
      outcomeEntryId: this.data.id
    };
    this.getData()
  }

  getData() {
    this.isLoading = true;
    forkJoin([
      this.currencyService.GetAllForDropdown().pipe(
        tap((currencyList) => {
          this.currencyList = currencyList.result;
        })
      ),
      this.requestService.IsOutcomingEntryHasDetail(this.data.id).pipe(
        tap((isOutcomingEntryHasDetail) => {
          this.isShowMoney = !isOutcomingEntryHasDetail.result;
        })
      )
    ]).subscribe({
      next: () => {},
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  clone() {
    this.isLoading = true
    this.requestService.cloneOutcomingEntry(this.newOutcomingEntry).subscribe(rs => {
      abp.notify.success("Clone successful")
      this.isLoading = false
      this.router.navigate(["/app/requestDetail/detail"], {
        queryParams: {
          id: rs.result
        }
      })
      this.dialogRef.close()
    },
      () => this.isLoading = false)
  }

}
