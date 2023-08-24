import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestChangeDialogComponent } from './request-change-dialog.component';

describe('RequestChangeDialogComponent', () => {
  let component: RequestChangeDialogComponent;
  let fixture: ComponentFixture<RequestChangeDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RequestChangeDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestChangeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
