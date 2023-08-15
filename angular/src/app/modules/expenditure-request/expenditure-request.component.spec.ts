import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenditureRequestComponent } from './expenditure-request.component';

describe('ExpenditureRequestComponent', () => {
  let component: ExpenditureRequestComponent;
  let fixture: ComponentFixture<ExpenditureRequestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpenditureRequestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpenditureRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
