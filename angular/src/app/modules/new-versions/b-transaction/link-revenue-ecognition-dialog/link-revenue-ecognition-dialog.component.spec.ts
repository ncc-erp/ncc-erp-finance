import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { LinkRevenueRecognitionAndBTransDialogComponent } from './link-revenue-ecognition-dialog.component';

describe('RevenueRecognitionDialogComponent', () => {
  let component: LinkRevenueRecognitionAndBTransDialogComponent;
  let fixture: ComponentFixture<LinkRevenueRecognitionAndBTransDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkRevenueRecognitionAndBTransDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkRevenueRecognitionAndBTransDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
