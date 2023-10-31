import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectionTreeCircleChartComponent } from './selection-tree-circle-chart.component';

describe('SelectionTreeCircleChartComponent', () => {
  let component: SelectionTreeCircleChartComponent;
  let fixture: ComponentFixture<SelectionTreeCircleChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SelectionTreeCircleChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectionTreeCircleChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
