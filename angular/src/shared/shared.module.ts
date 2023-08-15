import { AngularEditorModule } from "@kolkov/angular-editor";
import { CommonModule } from "@angular/common";
import { NgModule, ModuleWithProviders, LOCALE_ID } from "@angular/core";
import { RouterModule } from "@angular/router";
import { NgxPaginationModule } from "ngx-pagination";

import { AppSessionService } from "./session/app-session.service";
import { AppUrlService } from "./nav/app-url.service";
import { AppAuthService } from "./auth/app-auth.service";
import { AppRouteGuard } from "./auth/auth-route-guard";
import { LocalizePipe } from "@shared/pipes/localize.pipe";

import { AbpPaginationControlsComponent } from "./components/pagination/abp-pagination-controls.component";
import { AbpValidationSummaryComponent } from "./components/validation/abp-validation.summary.component";
import { AbpModalHeaderComponent } from "./components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from "./components/modal/abp-modal-footer.component";
import { LayoutStoreService } from "./layout/layout-store.service";

import { BusyDirective } from "./directives/busy.directive";
import { EqualValidator } from "./directives/equal-validator.directive";

//import angular material
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatBadgeModule } from "@angular/material/badge";
import { MatBottomSheetModule } from "@angular/material/bottom-sheet";
import { MatButtonModule } from "@angular/material/button";
import { MatButtonToggleModule } from "@angular/material/button-toggle";
import { MatCardModule } from "@angular/material/card";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatChipsModule } from "@angular/material/chips";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatDialogModule } from "@angular/material/dialog";
import { MatDividerModule } from "@angular/material/divider";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatMenuModule } from "@angular/material/menu";
import {
    DateAdapter,
    MatNativeDateModule,
    MAT_DATE_FORMATS,
    MAT_DATE_LOCALE,
} from "@angular/material/core";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatRadioModule } from "@angular/material/radio";
// import {MatSelectModule} from '@angular/material/select'
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSliderModule } from "@angular/material/slider";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatSortModule } from "@angular/material/sort";
import { MatStepperModule } from "@angular/material/stepper";
import { MatTableModule } from "@angular/material/table";
import { MatTabsModule } from "@angular/material/tabs";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatTreeModule } from "@angular/material/tree";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";
import { TextEditorComponent } from "./components/text-editor/text-editor.component";
import { DateSelectorComponent } from "./date-selector/date-selector.component";
import { PopupComponent } from "./date-selector/popup/popup.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DropdownFilterPipe } from "./pipes/dropdown-filter.pipe";
import { FormatMoneyPipe } from "./pipes/format-money.pipe";
import { CUSTOM_DATE_FORMATS, NGX_CUSTOM_DATE_FORMATS } from "./AppConsts";
import { MomentDateAdapter } from "@angular/material-moment-adapter";
import { NgxMatMomentModule } from '@angular-material-components/moment-adapter';
import { NgxMatDatetimePickerModule, NgxMatTimepickerModule, NGX_MAT_DATE_FORMATS } from "@angular-material-components/datetime-picker";
import {DragDropModule} from '@angular/cdk/drag-drop';
import { DmyDateFormatPipe } from './pipes/dmy-date-format.pipe';
import { SortableComponent } from './components/sortable/sortable.component';
import { MMYYDateFormatDirective } from './directives/mm-yy-date-format.directive';
import { CustomeSelectComponent } from './components/custome-select/custome-select.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { SelectionCustomsComponent } from './selection-customs/selection-customs.component';
import { SelectionTreeIncomingEntryTypeComponent } from "@app/modules/new-versions/b-transaction/create-multi-incoming-entry/selection-tree-incoming-entry-type/selection-tree-incoming-entry-type.component";
import { MyDateFormatPipe } from './pipes/my-date-format.pipe';
import { TreeInOutTypeComponent } from './components/tree-in-out-type/tree-in-out-type.component';


// import {DialogComponentModule} from './dialog-component/dialog-component.module';
// import { ErrorPermissionComponent } from './interceptor-errors/error-permission/error-permission.component'

@NgModule({
    imports: [
        ReactiveFormsModule,
        CommonModule,
        RouterModule,
        NgxPaginationModule,
        MatAutocompleteModule,
        MatBadgeModule,
        MatBottomSheetModule,
        MatButtonModule,
        MatButtonToggleModule,
        MatCardModule,
        MatCheckboxModule,
        MatChipsModule,
        MatDatepickerModule,
        MatDialogModule,
        MatDividerModule,
        MatExpansionModule,
        MatGridListModule,
        MatIconModule,
        MatInputModule,
        MatListModule,
        MatMenuModule,
        MatNativeDateModule,
        MatPaginatorModule,
        MatProgressBarModule,
        MatProgressSpinnerModule,
        MatRadioModule,
        MatSidenavModule,
        MatSliderModule,
        MatSlideToggleModule,
        MatSnackBarModule,
        MatSortModule,
        MatStepperModule,
        MatTableModule,
        MatTabsModule,
        MatToolbarModule,
        MatTooltipModule,
        MatTreeModule,
        MatFormFieldModule,
        MatSelectModule,
        AngularEditorModule,
        FormsModule,
        NgxMatDatetimePickerModule,
        NgxMatTimepickerModule,
        NgxMatMomentModule,
        DragDropModule,
        NgxMatSelectSearchModule,
    ],
    declarations: [
        AbpPaginationControlsComponent,
        AbpValidationSummaryComponent,
        AbpModalHeaderComponent,
        AbpModalFooterComponent,
        LocalizePipe,
        BusyDirective,
        EqualValidator,
        TextEditorComponent,
        DateSelectorComponent,
        PopupComponent,
        DropdownFilterPipe,
        FormatMoneyPipe,
        DmyDateFormatPipe,
        SortableComponent,
        MMYYDateFormatDirective,
        CustomeSelectComponent,
        SelectionCustomsComponent,
        SelectionTreeIncomingEntryTypeComponent,
        MyDateFormatPipe,
        TreeInOutTypeComponent
    ],
    exports: [
        TextEditorComponent,
        AbpPaginationControlsComponent,
        AbpValidationSummaryComponent,
        AbpModalHeaderComponent,
        AbpModalFooterComponent,
        LocalizePipe,
        BusyDirective,
        EqualValidator,
        MatAutocompleteModule,
        MatBadgeModule,
        MatBottomSheetModule,
        MatButtonModule,
        MatButtonToggleModule,
        MatCardModule,
        MatCheckboxModule,
        MatChipsModule,
        MatDatepickerModule,
        MatDialogModule,
        MatDividerModule,
        MatExpansionModule,
        MatGridListModule,
        MatIconModule,
        MatInputModule,
        MatListModule,
        MatMenuModule,
        MatNativeDateModule,
        MatPaginatorModule,
        MatProgressBarModule,
        MatProgressSpinnerModule,
        MatRadioModule,
        MatSelectModule,
        MatSidenavModule,
        MatSliderModule,
        MatSlideToggleModule,
        MatSnackBarModule,
        MatSortModule,
        MatStepperModule,
        MatTableModule,
        MatTabsModule,
        MatToolbarModule,
        MatTooltipModule,
        MatTreeModule,
        MatFormFieldModule,
        DateSelectorComponent,
        DropdownFilterPipe,
        FormsModule,
        FormatMoneyPipe,
        NgxMatDatetimePickerModule,
        NgxMatTimepickerModule,
        NgxMatMomentModule,
        DragDropModule,
        DmyDateFormatPipe,
        SortableComponent,
        MMYYDateFormatDirective,
        CustomeSelectComponent,
        ReactiveFormsModule,
        NgxMatSelectSearchModule,
        SelectionCustomsComponent,
        SelectionTreeIncomingEntryTypeComponent,
        TreeInOutTypeComponent,
        MyDateFormatPipe
    ],
    entryComponents: [PopupComponent],
    providers: [
        {
            provide: DateAdapter,
            useClass: MomentDateAdapter,
            deps: [MAT_DATE_LOCALE],
        },
        {
            provide: MAT_DATE_FORMATS,
            useValue: CUSTOM_DATE_FORMATS,
        },
        { provide: LOCALE_ID, useValue: 'en-GB' },
        { provide: NGX_MAT_DATE_FORMATS, useValue: NGX_CUSTOM_DATE_FORMATS },
    ],
})
export class SharedModule {
    static forRoot(): ModuleWithProviders<SharedModule> {
        return {
            ngModule: SharedModule,
            providers: [
                AppSessionService,
                AppUrlService,
                AppAuthService,
                AppRouteGuard,
                LayoutStoreService,
            ],
        };
    }
}
