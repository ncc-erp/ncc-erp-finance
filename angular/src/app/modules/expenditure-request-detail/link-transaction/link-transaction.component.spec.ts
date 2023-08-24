import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkTransactionComponent } from './link-transaction.component';

describe('LinkTransactionComponent', () => {
  let component: LinkTransactionComponent;
  let fixture: ComponentFixture<LinkTransactionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkTransactionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkTransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
