import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { LinkExpenditureAndBTransDialogComponent } from './link-expenditure-dialog.component';

describe('LinkExpenditureAndBTransDialogComponent', () => {
  let component: LinkExpenditureAndBTransDialogComponent;
  let fixture: ComponentFixture<LinkExpenditureAndBTransDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkExpenditureAndBTransDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkExpenditureAndBTransDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
