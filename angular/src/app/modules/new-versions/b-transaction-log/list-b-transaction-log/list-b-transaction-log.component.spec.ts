import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListBTransactionLogComponent } from './list-b-transaction-log.component';

describe('ListBTransactionLogComponent', () => {
  let component: ListBTransactionLogComponent;
  let fixture: ComponentFixture<ListBTransactionLogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListBTransactionLogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListBTransactionLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
