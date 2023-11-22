import { Component, EventEmitter, Inject, Injector, Input, OnChanges, OnInit, Optional, Output, SimpleChanges } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { APP_CONSTANT } from '@app/constant/api.constant';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { DATE_TIME_OPTIONS, DATE_TIME_OPTIONS_HOME } from '@shared/AppConsts';
import { DateSelectorHomeEnum } from '@shared/AppEnums';
import { AppComponentBase } from '@shared/app-component-base';
import { PopupComponent } from '@shared/date-selector/popup/popup.component';
import * as moment from 'moment';

@Component({
  selector: 'app-date-selector-dashboard',
  templateUrl: './date-selector-dashboard.component.html',
  styleUrls: ['./date-selector-dashboard.component.css']
})
export class DateSelectorDashboardComponent extends AppComponentBase implements OnInit, OnChanges
{
  customizeView: number = 0;
  isFistHalfYear: boolean = true;
  initOptionHalfYear: boolean = true;
  dateType: DateSelectorHomeEnum;
  dateText: string;
  isBtnPrev: boolean;
  currentDate = moment();

  searchDay = moment();

  public readonly DateSelectorHomeEnum = DateSelectorHomeEnum;
  @Input() viewChange: FormControl;
  @Input() type: DateSelectorHomeEnum;
  @Input() dateTimeOptions = DATE_TIME_OPTIONS_HOME;
  @Input() defaultDateFilter?: DateTimeSelectorHome
  @Output() onDateSelectorChange: EventEmitter<DateTimeSelectorHome> =
    new EventEmitter<DateTimeSelectorHome>();

  distanceFromAndToDate: string;

  defaultDateType: DateSelectorHomeEnum;

  constructor(
    injector: Injector,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
    private _dialog: MatDialog
  ) {
    super(injector);
  }

  ngOnInit() {
    this.defaultDateType = this.type;
    this.changeOptions(true);
  }

  ngOnChanges(change: SimpleChanges) {
    if (change["type"]) {
      this.defaultDateType = change["type"].currentValue;
    }
    if(change["defaultDateFilter"]){
      this.dateText = this.getDisplayText(this.defaultDateFilter.fromDate, this.defaultDateFilter.toDate);
    }
  }

  changeOptions(reset?: boolean) {
    if (reset) {
      this.customizeView = 0;
    }
    const { unitOfTime, keyOfTime, typeDate } = this.getUnitOfTime(
      this.defaultDateType
    );
    this.dateType = typeDate;
    if (this.defaultDateType === DateSelectorHomeEnum.HALF_YEAR) {
      this.selectOptionHalfYear();
      return;
    }
    if (this.defaultDateType == DateSelectorHomeEnum.CUSTOM) {
      this.showPopup();
      return;
    }
    const fromDate = moment()
      .clone()
      .startOf(unitOfTime)
      .add(this.customizeView, keyOfTime);
    const toDate = moment(fromDate).clone().endOf(unitOfTime);
    this.dateText = this.getDisplayText(fromDate, toDate);
    this.onDateSelectorChange.emit({
      dateType: this.dateType,
      fromDate,
      toDate,
      dateText: this.dateText,
    });
    return;
  }

  handleSelectDay() {
    if (this.searchDay) {
      this.onDateSelectorChange.emit({
        dateType: this.dateType,
        fromDate: this.searchDay,
        toDate: this.searchDay,
        dateText: moment(this.searchDay).format(DateFormat.DD_MM_YYYY),
      });
    }
  }

  nextOrPre(title: any) {
    if (this.defaultDateType === DateSelectorHomeEnum.CUSTOM) {
      return;
    }
    if (title === "pre") {
      this.customizeView--;
      this.isBtnPrev = true;
    }
    if (title === "next") {
      this.customizeView++;
      this.isBtnPrev = false;
    }
    this.changeOptions();
  }

  showPopup(): void {
    const popup = this._dialog.open(PopupComponent);
    popup.afterClosed().subscribe((res) => {
      if (res === undefined) return;
      if (res.result) {
        this.dateText =
          this.getDisplayText(
            res?.data.fromDateCustomTime,
            res?.data.toDateCustomTime
          ) || "No date select";
        this.onDateSelectorChange.emit({
          dateType: this.dateType,
          fromDate: res?.data.fromDateCustomTime,
          toDate: res?.data.toDateCustomTime,
          dateText: this.dateText,
        });
      }
      this._dialog.closeAll();
    });
  }

  private selectOptionHalfYear() {
    let fromDate;
    let toDate;
    if (this.initOptionHalfYear) {
      fromDate = moment().startOf("year");
      toDate = moment(fromDate).endOf("year").add(-6, "months");
      if (moment().quarter() > 2) {
        fromDate = moment().startOf("year").add(6, "months");
        toDate = moment(fromDate).endOf("year");
        this.isFistHalfYear = false;
      }
      this.initOptionHalfYear = false;
    } else {
      if (this.isFistHalfYear) {
        if (this.isBtnPrev) {
          this.currentDate = this.currentDate.add(-1, "years");
        }
        fromDate = this.currentDate.startOf("year").add(6, "months");
        toDate = moment(fromDate).endOf("year");
        this.isFistHalfYear = false;
      } else {
        if (this.isBtnPrev === false) {
          this.currentDate = this.currentDate.add(1, "years");
        }
        fromDate = this.currentDate.startOf("year");
        toDate = moment(fromDate).endOf("year").add(-6, "months");
        this.isFistHalfYear = true;
      }
    }

    this.dateText = this.getDisplayText(
      fromDate,
      toDate
    );
    this.onDateSelectorChange.emit({
      dateType: this.dateType,
      fromDate,
      toDate,
      dateText: this.dateText,
    });
  }

  private getUnitOfTime(type: DateSelectorHomeEnum): any {
    switch (type) {
      case DateSelectorHomeEnum.MONTH: {
        return {
          unitOfTime: DateSelectorHomeEnum.MONTH.toLowerCase(),
          keyOfTime: DateSelectorHomeEnum.MONTH.toLowerCase(),
          typeDate: DateSelectorHomeEnum.MONTH,
        };
      }
      case DateSelectorHomeEnum.QUARTER: {
        return {
          unitOfTime: DateSelectorHomeEnum.QUARTER.toLowerCase(),
          keyOfTime: DateSelectorHomeEnum.QUARTER.toLowerCase(),
          typeDate: DateSelectorHomeEnum.QUARTER,
        };
      }
      case DateSelectorHomeEnum.HALF_YEAR: {
        return {
          unitOfTime: DateSelectorHomeEnum.HALF_YEAR.toLowerCase(),
          keyOfTime: DateSelectorHomeEnum.HALF_YEAR.toLowerCase(),
          typeDate: DateSelectorHomeEnum.HALF_YEAR,
        };
      }
      case DateSelectorHomeEnum.YEAR: {
        return {
          unitOfTime: DateSelectorHomeEnum.YEAR.toLowerCase(),
          keyOfTime: DateSelectorHomeEnum.YEAR.toLowerCase(),
          typeDate: DateSelectorHomeEnum.YEAR,
        };
      }
      case DateSelectorHomeEnum.CUSTOM: {
        return {
          unitOfTime: DateSelectorHomeEnum.CUSTOM.toLowerCase(),
          keyOfTime: DateSelectorHomeEnum.CUSTOM.toLowerCase(),
          typeDate: DateSelectorHomeEnum.CUSTOM,
        };
      }
    }
  }

  private getDisplayText(
    fromDate: moment.Moment = null,
    toDate: moment.Moment = null
  ) {
    const spaceFormat = " - ";
    if (!fromDate || !toDate) {
      return "";
    }
    return `${fromDate.format(
      DateFormat.DD_MM_YYYY
    )}${spaceFormat}${toDate.format(DateFormat.DD_MM_YYYY)}`
  }
}

export interface DateTimeSelectorHome {
  dateType: DateSelectorHomeEnum;
  fromDate: moment.Moment;
  toDate: moment.Moment;
  dateText: string;
}


export const DateFormat = {
  YYYY_MM_DD: "YYYY/MM/DD",
  YYYY_MM_DD_H_MM_SS: "YYYY/MM/DD h:mm:ss",
  YYYY_MM_DD_HH_MM_SS: "YYYY/MM/DD H:mm:ss",
  DD_MM_YYYY: "DD/MM/YYYY",
  DD_MM_YYYY_H_MM: "DD/MM/YYYY H:mm",
  H_MM_SS: "h:mm:ss",
  MM_YYYY: "MM/YYYY",
  DD_MM: "DD/MM",
  YYYY: "YYYY",
  DD: "DD",
  MM: "MM",
};

