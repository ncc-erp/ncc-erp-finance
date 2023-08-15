import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LineChartSettingComponent } from './line-chart-setting.component';

describe('LineChartSettingComponent', () => {
  let component: LineChartSettingComponent;
  let fixture: ComponentFixture<LineChartSettingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LineChartSettingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LineChartSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
