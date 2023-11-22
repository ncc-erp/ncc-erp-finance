import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DateSelectorDashboardComponent } from './date-selector-dashboard.component';

describe('DateSelectorDashboardComponent', () => {
  let component: DateSelectorDashboardComponent;
  let fixture: ComponentFixture<DateSelectorDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DateSelectorDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DateSelectorDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
