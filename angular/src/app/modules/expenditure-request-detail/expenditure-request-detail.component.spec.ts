import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenditureRequestDetailComponent } from './expenditure-request-detail.component';

describe('ExpenditureRequestDetailComponent', () => {
  let component: ExpenditureRequestDetailComponent;
  let fixture: ComponentFixture<ExpenditureRequestDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpenditureRequestDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpenditureRequestDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
