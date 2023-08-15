import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkMultiBtransactionOutcomingEntryDialogComponent } from './link-multi-btransaction-outcoming-entry-dialog.component';

describe('LinkMultiBtransactionOutcomingEntryDialogComponent', () => {
  let component: LinkMultiBtransactionOutcomingEntryDialogComponent;
  let fixture: ComponentFixture<LinkMultiBtransactionOutcomingEntryDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkMultiBtransactionOutcomingEntryDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkMultiBtransactionOutcomingEntryDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
