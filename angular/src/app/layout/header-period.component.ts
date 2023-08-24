import { PeriodDto } from './../service/model/period.dto';
import { Component, OnInit } from '@angular/core';
import { PeriodService } from '@app/service/api/period.service';
import { AppConsts } from '@shared/AppConsts';
import { NavigationEnd, NavigationStart, Router } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'header-period',
  templateUrl: './header-period.component.html',
  styleUrls: ['./header-period.component.css']
})
export class HeaderPeriodComponent implements OnInit {

  public listPeriod: PeriodDto[] = []
  public selectedPeriodId: number
  public isShowFilter: boolean = false
  public isDisableFilter: boolean = false
  private listAllowFilter: string[] = ["btransaction", "finance-review", "bank-transaction", "expenditure-request", "revenue-record", "home", "requestDetail", "bankAccountDetail"]
  private notAllowChanges: string[] = ["requestDetail"]
  public tooltip: string = ""
  constructor(private periodService: PeriodService, private router: Router, private datePipe: DatePipe) {
    this.selectedPeriodId = AppConsts.periodId.getValue()
    this.isShowFilter = this.listAllowFilter.some(x => this.router.url.includes(x))
    this.isDisableFilter = this.notAllowChanges.some(x => this.router.url.includes(x))
  }

  ngOnInit(): void {
    AppConsts.periodId.asObservable().subscribe(rs => {
      if (rs == AppConsts.RefreshPeriod) {
        this.periodService.getAll().subscribe(result => {
          this.listPeriod = result.result
          let lastestPeriodId = this.listPeriod[0].id
          AppConsts.periodId.next(lastestPeriodId)
          this.selectedPeriodId = lastestPeriodId
        })
      }
      if(rs == AppConsts.GetFirstPeriod){
        this.periodService.getFirstPeriod().subscribe(result => {
          if(result.success && result.result){
            AppConsts.periodId.next(result.result.id)
            this.selectedPeriodId = AppConsts.periodId.getValue()

          }
        })
      }
    })

    this.router.events
      .subscribe((event: NavigationEnd) => {
        if (event instanceof NavigationEnd) {
          this.isShowFilter = this.listAllowFilter.some(x => event.url.includes(x))
          this.isDisableFilter = this.notAllowChanges.some(x => this.router.url.includes(x))
        }
      });

    this.getAllPeriod()
  }

  private getAllPeriod() {
    this.periodService.getAll().subscribe(rs => {
      this.listPeriod = rs.result
      this.listPeriod.forEach(x => {
        this.tooltip += `- ${x.name} (${this.formatDate(x.startDate)} ${x.endDate ? '- ' + this.formatDate(x.endDate) : ""}) ${x.statusName} \n`
      })
    })
  }

  private formatDate(date: string) {
    return date ? this.datePipe.transform(date, "dd/MM/yyyy") : ""
  }

  public onPeriodChange() {
    AppConsts.periodId.next(this.selectedPeriodId)
  }
}
