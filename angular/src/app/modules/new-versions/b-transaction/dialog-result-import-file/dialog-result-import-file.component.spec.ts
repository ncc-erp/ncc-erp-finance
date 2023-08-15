import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogResultImportFileComponent } from './dialog-result-import-file.component';

describe('DialogResultImportFileComponent', () => {
  let component: DialogResultImportFileComponent;
  let fixture: ComponentFixture<DialogResultImportFileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DialogResultImportFileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogResultImportFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
