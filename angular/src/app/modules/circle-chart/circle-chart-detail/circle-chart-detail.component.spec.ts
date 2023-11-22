import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CircleChartDetailComponent } from './circle-chart-detail.component';

describe('CircleChartDetailComponent', () => {
  let component: CircleChartDetailComponent;
  let fixture: ComponentFixture<CircleChartDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CircleChartDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CircleChartDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
