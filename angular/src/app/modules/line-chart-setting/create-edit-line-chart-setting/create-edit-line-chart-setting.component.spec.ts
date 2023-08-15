import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditLineChartSettingComponent } from './create-edit-line-chart-setting.component';

describe('CreateEditLineChartSettingComponent', () => {
  let component: CreateEditLineChartSettingComponent;
  let fixture: ComponentFixture<CreateEditLineChartSettingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditLineChartSettingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditLineChartSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
