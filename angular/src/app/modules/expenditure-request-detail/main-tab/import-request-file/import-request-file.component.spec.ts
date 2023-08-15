import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportRequestFileComponent } from './import-request-file.component';

describe('ImportRequestFileComponent', () => {
  let component: ImportRequestFileComponent;
  let fixture: ComponentFixture<ImportRequestFileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImportRequestFileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportRequestFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
