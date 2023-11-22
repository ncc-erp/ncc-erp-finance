import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditCircleChartComponent } from './create-edit-circle-chart.component';

describe('CreateEditCircleChartComponent', () => {
  let component: CreateEditCircleChartComponent;
  let fixture: ComponentFixture<CreateEditCircleChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditCircleChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditCircleChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
