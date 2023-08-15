import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenditureListComponent } from './expenditure-list.component';

describe('ExpenditureListComponent', () => {
  let component: ExpenditureListComponent;
  let fixture: ComponentFixture<ExpenditureListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpenditureListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpenditureListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
