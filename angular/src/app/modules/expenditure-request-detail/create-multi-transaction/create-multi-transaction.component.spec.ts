import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMultiTransactionComponent } from './create-multi-transaction.component';

describe('CreateMultiTransactionComponent', () => {
  let component: CreateMultiTransactionComponent;
  let fixture: ComponentFixture<CreateMultiTransactionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateMultiTransactionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateMultiTransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
