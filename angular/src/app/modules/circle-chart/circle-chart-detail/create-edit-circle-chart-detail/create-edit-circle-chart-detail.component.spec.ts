import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditCircleChartDetailComponent } from './create-edit-circle-chart-detail.component';

describe('CreateEditCircleChartDetailComponent', () => {
  let component: CreateEditCircleChartDetailComponent;
  let fixture: ComponentFixture<CreateEditCircleChartDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditCircleChartDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditCircleChartDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
